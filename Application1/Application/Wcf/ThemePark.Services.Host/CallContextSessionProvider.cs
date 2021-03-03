using System.Runtime.Remoting.Messaging;
using Abp.Runtime.Session;
using ThemePark.Infrastructure.Services;

namespace ThemePark.Services.Host
{
    public class CallContextSessionProvider : ISessionProvider
    {
        private IAbpSession _abpSession;

        public IAbpSession GetCurrentSession()
        {
            if (_abpSession == null)
            {
                //_abpSession = new WcfSession(
                //    GetData<long?>(SessionKey.UserId),
                //    GetData<int?>(SessionKey.TenantId),
                //    GetData<long?>(SessionKey.ImpersonatorUserId),
                //    GetData<int?>(SessionKey.ImpersonatorTenantId));
            }

            return _abpSession;
        }

        private T GetData<T>(string key)
        {
            return (T)CallContext.LogicalGetData(key);
        }
    }
}
