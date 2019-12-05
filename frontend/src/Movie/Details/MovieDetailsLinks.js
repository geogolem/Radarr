import PropTypes from 'prop-types';
import React from 'react';
import Label from 'Components/Label';
import Link from 'Components/Link/Link';
import { kinds, sizes } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import styles from './MovieDetailsLinks.css';

function MovieDetailsLinks(props) {
  const {
    tmdbId,
    imdbId,
    youTubeTrailerId,
    justwatchUrl,
	netflixUrl,
	primeVideoUrl,
	tubiTVUrl,
	hooplaUrl
  } = props;

  return (
    <div className={styles.links}>
      <Link
        className={styles.link}
        to={`https://www.themoviedb.org/movie/${tmdbId}`}
      >
        <Label
          className={styles.linkLabel}
          kind={kinds.INFO}
          size={sizes.LARGE}
        >
          {translate('TMDb')}
        </Label>
      </Link>

      <Link
        className={styles.link}
        to={`https://trakt.tv/search/tmdb/${tmdbId}?id_type=movie`}
      >
        <Label
          className={styles.linkLabel}
          kind={kinds.INFO}
          size={sizes.LARGE}
        >
          {translate('Trakt')}
        </Label>
      </Link>

      {
        !!imdbId &&
          <Link
            className={styles.link}
            to={`https://imdb.com/title/${imdbId}/`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.INFO}
              size={sizes.LARGE}
            >
              {translate('IMDb')}
            </Label>
          </Link>
      }

      {
        !!imdbId &&
          <Link
            className={styles.link}
            to={` https://moviechat.org/${imdbId}/`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.INFO}
              size={sizes.LARGE}
            >
              {translate('MovieChat')}
            </Label>
          </Link>
      }

      {
        !!youTubeTrailerId &&
          <Link
            className={styles.link}
            to={` https://www.youtube.com/watch?v=${youTubeTrailerId}/`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.DANGER}
              size={sizes.LARGE}
            >
              {translate('Trailer')}
            </Label>
          </Link>
      }

      {
        !!justwatchUrl &&
          <Link
            className={styles.link}
            to={` ${justwatchUrl}`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.INVERSE}
              size={sizes.LARGE}
            >
              Justwatch
            </Label>
          </Link>
      }
  
      {
        !!netflixUrl &&
          <Link
            className={styles.link}
            to={` ${netflixUrl}`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.SUCCESS}
              size={sizes.LARGE}
            >
              Netflix
            </Label>
          </Link>
      }

      {
        !!primeVideoUrl &&
          <Link
            className={styles.link}
            to={` ${primeVideoUrl}`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.SUCCESS}
              size={sizes.LARGE}
            >
              PrimeVideo
            </Label>
          </Link>
      }

      {
        !!tubiTVUrl &&
          <Link
            className={styles.link}
            to={` ${tubiTVUrl}`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.SUCCESS}
              size={sizes.LARGE}
            >
              TubiTV
            </Label>
          </Link>
      }

      {
        !!hooplaUrl &&
          <Link
            className={styles.link}
            to={` ${hooplaUrl}`}
          >
            <Label
              className={styles.linkLabel}
              kind={kinds.SUCCESS}
              size={sizes.LARGE}
            >
              Hoopla
            </Label>
          </Link>
      }
    </div>
  );
}

MovieDetailsLinks.propTypes = {
  tmdbId: PropTypes.number.isRequired,
  imdbId: PropTypes.string,
  youTubeTrailerId: PropTypes.string,
  justwatchUrl: PropTypes.string,
  netflixUrl: PropTypes.string,
  primeVideoUrl: PropTypes.string,
  tubiTVUrl: PropTypes.string,
  hooplaUrl: PropTypes.string
};

export default MovieDetailsLinks;
