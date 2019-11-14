using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using NLog;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Instrumentation.Extensions;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Exceptions;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.Movies.AlternativeTitles;
using NzbDrone.Core.Movies.Commands;
using NzbDrone.Core.Movies.Credits;
using NzbDrone.Core.Movies.Events;
using NzbDrone.Core.Movies.Translations;

namespace NzbDrone.Core.Movies
{
    public class RefreshMovieService : IExecute<RefreshMovieCommand>
    {
        private readonly IProvideMovieInfo _movieInfo;
        private readonly IMovieService _movieService;
        private readonly IMovieTranslationService _movieTranslationService;
        private readonly IAlternativeTitleService _titleService;
        private readonly ICreditService _creditService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDiskScanService _diskScanService;
        private readonly ICheckIfMovieShouldBeRefreshed _checkIfMovieShouldBeRefreshed;
        private readonly IConfigService _configService;

        private readonly Logger _logger;

        public RefreshMovieService(IProvideMovieInfo movieInfo,
                                    IMovieService movieService,
                                    IMovieTranslationService movieTranslationService,
                                    IAlternativeTitleService titleService,
                                    ICreditService creditService,
                                    IEventAggregator eventAggregator,
                                    IDiskScanService diskScanService,
                                    ICheckIfMovieShouldBeRefreshed checkIfMovieShouldBeRefreshed,
                                    IConfigService configService,
                                    Logger logger)
        {
            _movieInfo = movieInfo;
            _movieService = movieService;
            _movieTranslationService = movieTranslationService;
            _titleService = titleService;
            _creditService = creditService;
            _eventAggregator = eventAggregator;
            _diskScanService = diskScanService;
            _checkIfMovieShouldBeRefreshed = checkIfMovieShouldBeRefreshed;
            _configService = configService;
            _logger = logger;
        }

        public class OmdbObject
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Rated { get; set; }
            public string Released { get; set; }
            public string Runtime { get; set; }
            public string Genre { get; set; }
            public string Director { get; set; }
            public string Writer { get; set; }
            public string Actors { get; set; }
            public string Plot { get; set; }
            public string Language { get; set; }
            public string Country { get; set; }
            public string Awards { get; set; }
            public string Poster { get; set; }
            public string Metascore { get; set; }
            public string imdbRating { get; set; }
            public string imdbVotes { get; set; }
            public string imdbID { get; set; }
            public string Type { get; set; }
            public string tomatoMeter { get; set; }
            public string tomatoImage { get; set; }
            public string tomatoRating { get; set; }
            public string tomatoReviews { get; set; }
            public string tomatoFresh { get; set; }
            public string tomatoRotten { get; set; }
            public string tomatoConsensus { get; set; }
            public string tomatoUserMeter { get; set; }
            public string tomatoUserRating { get; set; }
            public string tomatoUserReviews { get; set; }
            public string tomatoURL { get; set; }
            public string DVD { get; set; }
            public string BoxOffice { get; set; }
            public string Production { get; set; }
            public string Website { get; set; }
            public string Response { get; set; }
        }

        public Tuple<DateTime?, DateTime?> determineReleaseDates(DateTime? tmdbInCinemas, DateTime? tmdbPhysicalRelease, string imdbId)
        {
            //Summary:
            // if THEMOVIEDB returns both a physical and cinemas date use those and stop
            // otherwise THEMOVIEDB returned partial or no information so check OMDBAPI
            // if OMDBAPI returns both a physical and cinema date use those and stop
            // at this pointif we are still going, both OMDBAPI and THEMOVIEDB each returned no or partial information
            // if both OMDBAPI and THEMOVIEDB returned partial information and the partial info doesnt overlap construct full information
            // if the full information constructed makes sense, use that and stop
            // at this point we know full information is not going to be available
            // if THEMOVIEDB had partial information, use it and stop
            // if OMDBAPI returns partial information use it and stop
            // if we got here both OMDBAPI and THEMOVIEDB return no information, no information is propagated and we stop.
            if (tmdbInCinemas.HasValue && tmdbPhysicalRelease.HasValue)
            {
                if (tmdbPhysicalRelease.Value.Subtract(tmdbInCinemas.Value).Duration().TotalSeconds < 60 * 60 * 24 * 15)
                {
                    tmdbPhysicalRelease = tmdbInCinemas;
                }

                if (tmdbInCinemas <= tmdbPhysicalRelease)
                {
                    return Tuple.Create(tmdbInCinemas, tmdbPhysicalRelease);
                }
            }

            //lets augment the releasedate information with info from omdbapi
            string data;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadString("http://www.omdbapi.com/?i=" + imdbId + "&tomatoes=true&plot=short&r=json&apikey=" + _configService.OmdbApiKey);
            }

            OmdbObject j1 = Newtonsoft.Json.JsonConvert.DeserializeObject<OmdbObject>(data);
            DateTime? omdbInCinemas;
            DateTime? omdbPhysicalRelease;
            if (j1.Released != "N/A")
            {
                omdbInCinemas = DateTime.Parse(j1.Released);
            }
            else
            {
                omdbInCinemas = null;
            }

            if (j1.DVD != "N/A")
            {
                omdbPhysicalRelease = DateTime.Parse(j1.DVD);
            }
            else
            {
                omdbPhysicalRelease = null;
            }

            if (omdbInCinemas.HasValue && omdbPhysicalRelease.HasValue)
            {
                if (omdbPhysicalRelease.Value.Subtract(omdbInCinemas.Value).Duration().TotalSeconds < 60 * 60 * 24 * 15)
                {
                    omdbPhysicalRelease = omdbInCinemas;
                }

                if (omdbInCinemas <= omdbPhysicalRelease)
                {
                    return Tuple.Create(omdbInCinemas, omdbPhysicalRelease);
                }
            }

            //now we know that we either have partial information or no information
            if (omdbInCinemas.HasValue && tmdbPhysicalRelease.HasValue)
            {
                if (omdbInCinemas <= tmdbPhysicalRelease)
                {
                    return Tuple.Create(omdbInCinemas, tmdbPhysicalRelease);
                }
            }

            if (omdbPhysicalRelease.HasValue && tmdbInCinemas.HasValue)
            {
                if (tmdbInCinemas <= omdbPhysicalRelease)
                {
                    return Tuple.Create(tmdbInCinemas, omdbPhysicalRelease);
                }
            }

            if ((omdbInCinemas.HasValue && !omdbPhysicalRelease.HasValue) || (!omdbInCinemas.HasValue && omdbPhysicalRelease.HasValue))
            {
                return Tuple.Create(omdbInCinemas, omdbPhysicalRelease);
            }
            else if ((tmdbInCinemas.HasValue && !tmdbPhysicalRelease.HasValue) || (!tmdbInCinemas.HasValue && tmdbPhysicalRelease.HasValue))
            {
                return Tuple.Create(tmdbInCinemas, tmdbPhysicalRelease);
            }

            if (omdbPhysicalRelease.HasValue)
            {
                omdbInCinemas = null;
                return Tuple.Create(omdbInCinemas, omdbPhysicalRelease);
            }
            else if (tmdbPhysicalRelease.HasValue)
            {
                tmdbInCinemas = null;
                return Tuple.Create(tmdbInCinemas, tmdbPhysicalRelease);
            }
            else if (omdbInCinemas.HasValue)
            {
                omdbPhysicalRelease = null;
                return Tuple.Create(omdbInCinemas, omdbPhysicalRelease);
            }
            else if (tmdbInCinemas.HasValue)
            {
                tmdbPhysicalRelease = null;
                return Tuple.Create(tmdbInCinemas, tmdbPhysicalRelease);
            }

            omdbInCinemas = null;
            omdbPhysicalRelease = null;
            return Tuple.Create(omdbInCinemas, omdbPhysicalRelease);
        }

        private Movie RefreshMovieInfo(int movieId)
        {
            // Get the movie before updating, that way any changes made to the movie after the refresh started,
            // but before this movie was refreshed won't be lost.
            var movie = _movieService.GetMovie(movieId);

            _logger.ProgressInfo("Updating info for {0}", movie.Title);

            Movie movieInfo;
            List<Credit> credits;

            try
            {
                var tuple = _movieInfo.GetMovieInfo(movie.TmdbId);
                movieInfo = tuple.Item1;
                credits = tuple.Item2;
            }
            catch (MovieNotFoundException)
            {
                if (movie.Status != MovieStatusType.Deleted)
                {
                    movie.Status = MovieStatusType.Deleted;
                    _movieService.UpdateMovie(movie);
                    _logger.Debug("Movie marked as deleted on TMDb for {0}", movie.Title);
                    _eventAggregator.PublishEvent(new MovieUpdatedEvent(movie));
                }

                throw;
            }

            if (movie.TmdbId != movieInfo.TmdbId)
            {
                _logger.Warn("Movie '{0}' (TMDb: {1}) was replaced with '{2}' (TMDb: {3}), because the original was a duplicate.", movie.Title, movie.TmdbId, movieInfo.Title, movieInfo.TmdbId);
                movie.TmdbId = movieInfo.TmdbId;
            }

            if (movieInfo.Year != 0 && _configService.OmdbApiKey.IsNotNullOrWhiteSpace())
            {
                Tuple<DateTime?, DateTime?> t = determineReleaseDates(movieInfo.InCinemas, movieInfo.PhysicalRelease, movieInfo.ImdbId);
                movieInfo.InCinemas = t.Item1;
                movieInfo.PhysicalRelease = t.Item2;
            }

            var now = DateTime.Now;

            movieInfo.Status = MovieStatusType.Announced;

            if (movieInfo.InCinemas.HasValue && now > movieInfo.InCinemas)
            {
                movieInfo.Status = MovieStatusType.InCinemas;

                if (!movieInfo.PhysicalRelease.HasValue && !movieInfo.DigitalRelease.HasValue && now > movieInfo.InCinemas.Value.AddDays(90))
                {
                    movieInfo.Status = MovieStatusType.Released;
                }
            }

            if (movieInfo.PhysicalRelease.HasValue && now >= movieInfo.PhysicalRelease)
            {
                movieInfo.Status = MovieStatusType.Released;
            }

            if (movieInfo.DigitalRelease.HasValue && now >= movieInfo.DigitalRelease)
            {
                movieInfo.Status = MovieStatusType.Released;
            }

            movie.Title = movieInfo.Title;
            movie.TitleSlug = movieInfo.TitleSlug;
            movie.ImdbId = movieInfo.ImdbId;
            movie.Overview = movieInfo.Overview;
            movie.Status = movieInfo.Status;
            movie.CleanTitle = movieInfo.CleanTitle;
            movie.SortTitle = movieInfo.SortTitle;
            movie.LastInfoSync = DateTime.UtcNow;
            movie.Runtime = movieInfo.Runtime;
            movie.Images = movieInfo.Images;
            movie.Ratings = movieInfo.Ratings;
            movie.Collection = movieInfo.Collection;
            movie.Genres = movieInfo.Genres;
            movie.Certification = movieInfo.Certification;
            movie.InCinemas = movieInfo.InCinemas;
            movie.Website = movieInfo.Website;
            movie.Year = movieInfo.Year;
            movie.SecondaryYear = movieInfo.SecondaryYear;
            movie.PhysicalRelease = movieInfo.PhysicalRelease;
            movie.DigitalRelease = movieInfo.DigitalRelease;
            movie.YouTubeTrailerId = movieInfo.YouTubeTrailerId;
            movie.Studio = movieInfo.Studio;
            movie.OriginalTitle = movieInfo.OriginalTitle;
            movie.OriginalLanguage = movieInfo.OriginalLanguage;
            movie.HasPreDBEntry = movieInfo.HasPreDBEntry;
            movie.Recommendations = movieInfo.Recommendations;

            try
            {
                movie.Path = new DirectoryInfo(movie.Path).FullName;
                movie.Path = movie.Path.GetActualCasing();
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Couldn't update movie path for " + movie.Path);
            }

            movie.AlternativeTitles = _titleService.UpdateTitles(movieInfo.AlternativeTitles, movie);
            _movieTranslationService.UpdateTranslations(movieInfo.Translations, movie);

            _movieService.UpdateMovie(new List<Movie> { movie }, true);
            _creditService.UpdateCredits(credits, movie);

            _logger.Debug("Finished movie refresh for {0}", movie.Title);
            _eventAggregator.PublishEvent(new MovieUpdatedEvent(movie));

            return movie;
        }

        private void RescanMovie(Movie movie, bool isNew, CommandTrigger trigger)
        {
            var rescanAfterRefresh = _configService.RescanAfterRefresh;
            var shouldRescan = true;

            if (isNew)
            {
                _logger.Trace("Forcing rescan of {0}. Reason: New movie", movie);
                shouldRescan = true;
            }
            else if (rescanAfterRefresh == RescanAfterRefreshType.Never)
            {
                _logger.Trace("Skipping rescan of {0}. Reason: Never rescan after refresh", movie);
                shouldRescan = false;
            }
            else if (rescanAfterRefresh == RescanAfterRefreshType.AfterManual && trigger != CommandTrigger.Manual)
            {
                _logger.Trace("Skipping rescan of {0}. Reason: Not after automatic scans", movie);
                shouldRescan = false;
            }

            if (!shouldRescan)
            {
                return;
            }

            try
            {
                _diskScanService.Scan(movie);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Couldn't rescan movie {0}", movie);
            }
        }

        public void Execute(RefreshMovieCommand message)
        {
            var trigger = message.Trigger;
            var isNew = message.IsNewMovie;
            _eventAggregator.PublishEvent(new MovieRefreshStartingEvent(message.Trigger == CommandTrigger.Manual));

            if (message.MovieIds.Any())
            {
                foreach (var movieId in message.MovieIds)
                {
                    var movie = _movieService.GetMovie(movieId);

                    try
                    {
                        movie = RefreshMovieInfo(movieId);
                        RescanMovie(movie, isNew, trigger);
                    }
                    catch (MovieNotFoundException)
                    {
                        _logger.Error("Movie '{0}' (TMDb {1}) was not found, it may have been removed from The Movie Database.", movie.Title, movie.TmdbId);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Couldn't refresh info for {0}", movie);
                        RescanMovie(movie, isNew, trigger);
                        throw;
                    }
                }
            }
            else
            {
                var allMovie = _movieService.GetAllMovies().OrderBy(c => c.SortTitle).ToList();

                var updatedTMDBMovies = new HashSet<int>();

                if (message.LastStartTime.HasValue && message.LastStartTime.Value.AddDays(14) > DateTime.UtcNow)
                {
                    updatedTMDBMovies = _movieInfo.GetChangedMovies(message.LastStartTime.Value);
                }

                foreach (var movie in allMovie)
                {
                    var movieLocal = movie;
                    if ((updatedTMDBMovies.Count == 0 && _checkIfMovieShouldBeRefreshed.ShouldRefresh(movie)) || updatedTMDBMovies.Contains(movie.TmdbId) || message.Trigger == CommandTrigger.Manual)
                    {
                        try
                        {
                            movieLocal = RefreshMovieInfo(movieLocal.Id);
                        }
                        catch (MovieNotFoundException)
                        {
                            _logger.Error("Movie '{0}' (TMDb {1}) was not found, it may have been removed from The Movie Database.", movieLocal.Title, movieLocal.TmdbId);
                            continue;
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, "Couldn't refresh info for {0}", movieLocal);
                        }

                        RescanMovie(movieLocal, false, trigger);
                    }
                    else
                    {
                        _logger.Debug("Skipping refresh of movie: {0}", movieLocal.Title);
                        RescanMovie(movieLocal, false, trigger);
                    }
                }
            }

            _eventAggregator.PublishEvent(new MovieRefreshCompleteEvent());
        }
    }
}
