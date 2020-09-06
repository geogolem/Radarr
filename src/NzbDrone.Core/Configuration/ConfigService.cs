using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NLog;
using NzbDrone.Common.EnsureThat;
using NzbDrone.Common.Http.Proxy;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Configuration.Events;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.MetadataSource.SkyHook.Resource;
using NzbDrone.Core.Qualities;
using NzbDrone.Core.Security;

namespace NzbDrone.Core.Configuration
{
    public enum ConfigKey
    {
        DownloadedMoviesFolder
    }

    public class ConfigService : IConfigService
    {
        private readonly IConfigRepository _repository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;
        private static Dictionary<string, string> _cache;

        public ConfigService(IConfigRepository repository, IEventAggregator eventAggregator, Logger logger)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _cache = new Dictionary<string, string>();
        }

        private Dictionary<string, object> AllWithDefaults()
        {
            var dict = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            var type = GetType();
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(this, null);
                dict.Add(propertyInfo.Name, value);
            }

            return dict;
        }

        public void SaveConfigDictionary(Dictionary<string, object> configValues)
        {
            var allWithDefaults = AllWithDefaults();

            foreach (var configValue in configValues)
            {
                object currentValue;
                allWithDefaults.TryGetValue(configValue.Key, out currentValue);
                if (currentValue == null || configValue.Value == null)
                {
                    continue;
                }

                var equal = configValue.Value.ToString().Equals(currentValue.ToString());

                if (!equal)
                {
                    SetValue(configValue.Key, configValue.Value.ToString());
                }
            }

            _eventAggregator.PublishEvent(new ConfigSavedEvent());
        }

        public bool IsDefined(string key)
        {
            return _repository.Get(key.ToLower()) != null;
        }

        public bool AutoUnmonitorPreviouslyDownloadedMovies
        {
            get { return GetValueBoolean("AutoUnmonitorPreviouslyDownloadedMovies"); }
            set { SetValue("AutoUnmonitorPreviouslyDownloadedMovies", value); }
        }

        public int Retention
        {
            get { return GetValueInt("Retention", 0); }
            set { SetValue("Retention", value); }
        }

        public string RecycleBin
        {
            get { return GetValue("RecycleBin", string.Empty); }
            set { SetValue("RecycleBin", value); }
        }

        public int RecycleBinCleanupDays
        {
            get { return GetValueInt("RecycleBinCleanupDays", 7); }
            set { SetValue("RecycleBinCleanupDays", value); }
        }

        public int RssSyncInterval
        {
            get { return GetValueInt("RssSyncInterval", 60); }

            set { SetValue("RssSyncInterval", value); }
        }

        public int AvailabilityDelay
        {
            get { return GetValueInt("AvailabilityDelay", 0); }
            set { SetValue("AvailabilityDelay", value); }
        }

        public int ImportListSyncInterval
        {
            get { return GetValueInt("ImportListSyncInterval", 24); }

            set { SetValue("ImportListSyncInterval", value); }
        }

        public string ListSyncLevel
        {
            get { return GetValue("ListSyncLevel", "disabled"); }
            set { SetValue("ListSyncLevel", value); }
        }

        public string ImportExclusions
        {
            get { return GetValue("ImportExclusions", string.Empty); }
            set { SetValue("ImportExclusions", value); }
        }

        public HashSet<int> CleanLibraryTags
        {
            get { return GetValueHashSet("CleanLibraryTags"); }
            set { SetValue("CleanLibraryTags", value); }
        }

        public TMDbCountryCode CertificationCountry
        {
            get { return GetValueEnum("CertificationCountry", TMDbCountryCode.US); }

            set { SetValue("CertificationCountry", value); }
        }

        public int MaximumSize
        {
            get { return GetValueInt("MaximumSize", 0); }
            set { SetValue("MaximumSize", value); }
        }

        public int MinimumAge
        {
            get { return GetValueInt("MinimumAge", 0); }

            set { SetValue("MinimumAge", value); }
        }

        public ProperDownloadTypes DownloadPropersAndRepacks
        {
            get { return GetValueEnum("DownloadPropersAndRepacks", ProperDownloadTypes.PreferAndUpgrade); }

            set { SetValue("DownloadPropersAndRepacks", value); }
        }

        public bool EnableCompletedDownloadHandling
        {
            get { return GetValueBoolean("EnableCompletedDownloadHandling", true); }

            set { SetValue("EnableCompletedDownloadHandling", value); }
        }

        public bool PreferIndexerFlags
        {
            get { return GetValueBoolean("PreferIndexerFlags", false); }

            set { SetValue("PreferIndexerFlags", value); }
        }

        public bool AllowHardcodedSubs
        {
            get { return GetValueBoolean("AllowHardcodedSubs", false); }

            set { SetValue("AllowHardcodedSubs", value); }
        }

        public string WhitelistedHardcodedSubs
        {
            get { return GetValue("WhitelistedHardcodedSubs", ""); }

            set { SetValue("WhitelistedHardcodedSubs", value); }
        }

        public bool RemoveCompletedDownloads
        {
            get { return GetValueBoolean("RemoveCompletedDownloads", false); }

            set { SetValue("RemoveCompletedDownloads", value); }
        }

        public bool AutoRedownloadFailed
        {
            get { return GetValueBoolean("AutoRedownloadFailed", true); }

            set { SetValue("AutoRedownloadFailed", value); }
        }

        public bool RemoveFailedDownloads
        {
            get { return GetValueBoolean("RemoveFailedDownloads", true); }

            set { SetValue("RemoveFailedDownloads", value); }
        }

        public bool CreateEmptyMovieFolders
        {
            get { return GetValueBoolean("CreateEmptyMovieFolders", false); }

            set { SetValue("CreateEmptyMovieFolders", value); }
        }

        public bool DeleteEmptyFolders
        {
            get { return GetValueBoolean("DeleteEmptyFolders", false); }

            set { SetValue("DeleteEmptyFolders", value); }
        }

        public FileDateType FileDate
        {
            get { return GetValueEnum("FileDate", FileDateType.None); }

            set { SetValue("FileDate", value); }
        }

        public string DownloadClientWorkingFolders
        {
            get { return GetValue("DownloadClientWorkingFolders", "_UNPACK_|_FAILED_"); }
            set { SetValue("DownloadClientWorkingFolders", value); }
        }

        public int CheckForFinishedDownloadInterval
        {
            get { return GetValueInt("CheckForFinishedDownloadInterval", 1); }

            set { SetValue("CheckForFinishedDownloadInterval", value); }
        }

        public int DownloadClientHistoryLimit
        {
            get { return GetValueInt("DownloadClientHistoryLimit", 60); }

            set { SetValue("DownloadClientHistoryLimit", value); }
        }

        public bool SkipFreeSpaceCheckWhenImporting
        {
            get { return GetValueBoolean("SkipFreeSpaceCheckWhenImporting", false); }

            set { SetValue("SkipFreeSpaceCheckWhenImporting", value); }
        }

        public int MinimumFreeSpaceWhenImporting
        {
            get { return GetValueInt("MinimumFreeSpaceWhenImporting", 100); }

            set { SetValue("MinimumFreeSpaceWhenImporting", value); }
        }

        public bool CopyUsingHardlinks
        {
            get { return GetValueBoolean("CopyUsingHardlinks", true); }

            set { SetValue("CopyUsingHardlinks", value); }
        }

        public bool EnableMediaInfo
        {
            get { return GetValueBoolean("EnableMediaInfo", true); }

            set { SetValue("EnableMediaInfo", value); }
        }

        public bool ImportExtraFiles
        {
            get { return GetValueBoolean("ImportExtraFiles", false); }

            set { SetValue("ImportExtraFiles", value); }
        }

        public string ExtraFileExtensions
        {
            get { return GetValue("ExtraFileExtensions", "srt"); }

            set { SetValue("ExtraFileExtensions", value); }
        }

        public bool AutoRenameFolders
        {
            get { return GetValueBoolean("AutoRenameFolders", false); }

            set { SetValue("AutoRenameFolders", value); }
        }

        public RescanAfterRefreshType RescanAfterRefresh
        {
            get { return GetValueEnum("RescanAfterRefresh", RescanAfterRefreshType.Always); }

            set { SetValue("RescanAfterRefresh", value); }
        }

        public bool SetPermissionsLinux
        {
            get { return GetValueBoolean("SetPermissionsLinux", false); }

            set { SetValue("SetPermissionsLinux", value); }
        }

        public string ChmodFolder
        {
            get { return GetValue("ChmodFolder", "755"); }

            set { SetValue("ChmodFolder", value); }
        }

        public string ChownGroup
        {
            get { return GetValue("ChownGroup", ""); }

            set { SetValue("ChownGroup", value); }
        }

        public int FirstDayOfWeek
        {
            get { return GetValueInt("FirstDayOfWeek", (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek); }

            set { SetValue("FirstDayOfWeek", value); }
        }

        public string CalendarWeekColumnHeader
        {
            get { return GetValue("CalendarWeekColumnHeader", "ddd M/D"); }

            set { SetValue("CalendarWeekColumnHeader", value); }
        }

        public MovieRuntimeFormatType MovieRuntimeFormat
        {
            get { return GetValueEnum("MovieRuntimeFormat", MovieRuntimeFormatType.HoursMinutes); }

            set { SetValue("MovieRuntimeFormat", value); }
        }

        public string ShortDateFormat
        {
            get { return GetValue("ShortDateFormat", "MMM D YYYY"); }

            set { SetValue("ShortDateFormat", value); }
        }

        public string LongDateFormat
        {
            get { return GetValue("LongDateFormat", "dddd, MMMM D YYYY"); }

            set { SetValue("LongDateFormat", value); }
        }

        public string TimeFormat
        {
            get { return GetValue("TimeFormat", "h(:mm)a"); }

            set { SetValue("TimeFormat", value); }
        }

        public bool ShowRelativeDates
        {
            get { return GetValueBoolean("ShowRelativeDates", true); }

            set { SetValue("ShowRelativeDates", value); }
        }

        public bool EnableColorImpairedMode
        {
            get { return GetValueBoolean("EnableColorImpairedMode", false); }

            set { SetValue("EnableColorImpairedMode", value); }
        }

        public int MovieInfoLanguage
        {
            get { return GetValueInt("MovieInfoLanguage", (int)Language.English); }

            set { SetValue("MovieInfoLanguage", value); }
        }

        public int UILanguage
        {
            get { return GetValueInt("UILanguage", (int)Language.English); }

            set { SetValue("UILanguage", value); }
        }

        public bool CleanupMetadataImages
        {
            get { return GetValueBoolean("CleanupMetadataImages", true); }

            set { SetValue("CleanupMetadataImages", value); }
        }

        public string PlexClientIdentifier => GetValue("PlexClientIdentifier", Guid.NewGuid().ToString(), true);

        public string RijndaelPassphrase => GetValue("RijndaelPassphrase", Guid.NewGuid().ToString(), true);

        public string HmacPassphrase => GetValue("HmacPassphrase", Guid.NewGuid().ToString(), true);

        public string RijndaelSalt => GetValue("RijndaelSalt", Guid.NewGuid().ToString(), true);

        public string HmacSalt => GetValue("HmacSalt", Guid.NewGuid().ToString(), true);

        public bool ProxyEnabled => GetValueBoolean("ProxyEnabled", false);

        public ProxyType ProxyType => GetValueEnum<ProxyType>("ProxyType", ProxyType.Http);

        public string ProxyHostname => GetValue("ProxyHostname", string.Empty);

        public int ProxyPort => GetValueInt("ProxyPort", 8080);

        public string ProxyUsername => GetValue("ProxyUsername", string.Empty);

        public string ProxyPassword => GetValue("ProxyPassword", string.Empty);

        public string ProxyBypassFilter => GetValue("ProxyBypassFilter", string.Empty);

        public bool ProxyBypassLocalAddresses => GetValueBoolean("ProxyBypassLocalAddresses", true);

        public string BackupFolder => GetValue("BackupFolder", "Backups");

        public int BackupInterval => GetValueInt("BackupInterval", 7);

        public int BackupRetention => GetValueInt("BackupRetention", 28);

        public CertificateValidationType CertificateValidation =>
            GetValueEnum("CertificateValidation", CertificateValidationType.Enabled);

        private string GetValue(string key)
        {
            return GetValue(key, string.Empty);
        }

        private bool GetValueBoolean(string key, bool defaultValue = false)
        {
            return Convert.ToBoolean(GetValue(key, defaultValue));
        }

        private int GetValueInt(string key, int defaultValue = 0)
        {
            return Convert.ToInt32(GetValue(key, defaultValue));
        }

        private HashSet<int> GetValueHashSet(string key, int defaultValue = 0)
        {
            var tags = new HashSet<int>();
            tags.Add(defaultValue);
            return tags;
        }

        private T GetValueEnum<T>(string key, T defaultValue)
        {
            return (T)Enum.Parse(typeof(T), GetValue(key, defaultValue), true);
        }

        public string GetValue(string key, object defaultValue, bool persist = false)
        {
            key = key.ToLowerInvariant();
            Ensure.That(key, () => key).IsNotNullOrWhiteSpace();

            EnsureCache();

            string dbValue;

            if (_cache.TryGetValue(key, out dbValue) && dbValue != null && !string.IsNullOrEmpty(dbValue))
            {
                return dbValue;
            }

            _logger.Trace("Using default config value for '{0}' defaultValue:'{1}'", key, defaultValue);

            if (persist)
            {
                SetValue(key, defaultValue.ToString());
            }

            return defaultValue.ToString();
        }

        private void SetValue(string key, bool value)
        {
            SetValue(key, value.ToString());
        }

        private void SetValue(string key, int value)
        {
            SetValue(key, value.ToString());
        }

        private void SetValue(string key, HashSet<int> value)
        {
            SetValue(key, value.ToString());
        }

        private void SetValue(string key, Enum value)
        {
            SetValue(key, value.ToString().ToLower());
        }

        private void SetValue(string key, string value)
        {
            key = key.ToLowerInvariant();

            _logger.Trace("Writing Setting to database. Key:'{0}' Value:'{1}'", key, value);
            _repository.Upsert(key, value);

            ClearCache();
        }

        private void EnsureCache()
        {
            lock (_cache)
            {
                if (!_cache.Any())
                {
                    var all = _repository.All();
                    _cache = all.ToDictionary(c => c.Key.ToLower(), c => c.Value);
                }
            }
        }

        private static void ClearCache()
        {
            lock (_cache)
            {
                _cache = new Dictionary<string, string>();
            }
        }
    }
}
