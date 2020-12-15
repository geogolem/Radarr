using NzbDrone.Core.Configuration;

namespace Radarr.Api.V3.Config
{
    public class JustwatchConfigModule : RadarrConfigModule<JustwatchConfigResource>
    {
        public JustwatchConfigModule(IConfigService configService)
            : base(configService)
        {
        }

        protected override JustwatchConfigResource ToResource(IConfigService model)
        {
            return JustwatchConfigResourceMapper.ToResource(model);
        }
    }
}
