using System;
using System.Runtime.Remoting.Messaging;
using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using ThemePark.Infrastructure.Services;

namespace ThemePark.Services.Host
{
    public class WcfSession : IAbpSession
    {
        protected readonly IMultiTenancyConfig MultiTenancy;

        /// <summary>初始化 <see cref="WcfSession" /> 类的新实例。</summary>
        public WcfSession(IMultiTenancyConfig multiTenancy)
        {
            MultiTenancy = multiTenancy;
        }

        /// <summary>
        /// Used to change <see cref="P:Abp.Runtime.Session.IAbpSession.TenantId" /> and <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" /> for a limited scope.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IDisposable Use(int? tenantId, long? userId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets current UserId or null.
        /// It can be null if no user logged in.
        /// </summary>
        public long? UserId
        {
            get { return GetData<long?>(SessionKey.UserId); }
        }

        /// <summary>
        /// Gets current TenantId or null.
        /// This TenantId should be the TenantId of the <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" />.
        /// It can be null if given <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" /> is a host user or no user logged in.
        /// </summary>
        public int? TenantId
        {
            get
            {
                return GetData<int?>(SessionKey.TenantId);
            }
        }

        /// <summary>Gets current multi-tenancy side.</summary>
        public MultiTenancySides MultiTenancySide
        {
            get
            {
                return MultiTenancy.IsEnabled && !TenantId.HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }
        }

        /// <summary>
        /// UserId of the impersonator.
        /// This is filled if a user is performing actions behalf of the <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" />.
        /// </summary>
        public long? ImpersonatorUserId
        {
            get
            {
                return GetData<long?>(SessionKey.ImpersonatorUserId);
            }
        }

        /// <summary>
        /// TenantId of the impersonator.
        /// This is filled if a user with <see cref="P:Abp.Runtime.Session.IAbpSession.ImpersonatorUserId" /> performing actions behalf of the <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" />.
        /// </summary>
        public int? ImpersonatorTenantId
        {
            get
            {
                return GetData<int?>(SessionKey.ImpersonatorTenantId);
            }
        }

        private T GetData<T>(string key)
        {
            return (T)CallContext.LogicalGetData(key);
        }
    }
}
