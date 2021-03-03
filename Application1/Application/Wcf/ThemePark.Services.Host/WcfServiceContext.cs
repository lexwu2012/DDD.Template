using ThemePark.Infrastructure.Services;

namespace ThemePark.Services.Host
{
    public class WcfServiceContext : ServiceContext
    {
        public WcfServiceContext(IServiceManager serviceManager) : base(serviceManager)
        {
        }
    }
}
