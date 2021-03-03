using Abp.Runtime.Session;

namespace ThemePark.Infrastructure.Services
{
    public interface ISessionProvider
    {
        IAbpSession GetCurrentSession();
    }
}
