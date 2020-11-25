using NzbDrone.Core.Configuration;
using Radarr.Http.REST;

namespace Radarr.Api.V3.Config
{
    public class JustwatchConfigResource : RestResource
    {
        public string JustwatchLocale { get; set; }
        public string EnableNetflix { get; set; }
        public bool IgnoreNetflixTitles { get; set; }
        public string EnablePrimeVideo { get; set; }
        public bool IgnorePrimeVideoTitles { get; set; }
        public string EnableTubiTV { get; set; }
        public bool IgnoreTubiTVTitles { get; set; }
        public string EnableHoopla { get; set; }
        public bool IgnoreHooplaTitles { get; set; }
    }

    public static class JustwatchConfigResourceMapper
    {
        public static JustwatchConfigResource ToResource(IConfigService model)
        {
            return new JustwatchConfigResource
            {
                JustwatchLocale = model.JustwatchLocale,
                EnableNetflix = model.EnableNetflix,
                IgnoreNetflixTitles = model.IgnoreNetflixTitles,
                EnablePrimeVideo = model.EnablePrimeVideo,
                IgnorePrimeVideoTitles = model.IgnorePrimeVideoTitles,
                EnableTubiTV = model.EnableTubiTV,
                IgnoreTubiTVTitles = model.IgnoreTubiTVTitles,
                EnableHoopla = model.EnableHoopla,
                IgnoreHooplaTitles = model.IgnoreHooplaTitles,
            };
        }
    }
}
