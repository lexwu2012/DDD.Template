using System;
using System.Runtime.Serialization;
using Abp.Runtime.Session;

namespace ThemePark.Infrastructure.Services
{
    [Serializable]
    [DataContract]
    public class SessionContext
    {
        public const string ContextName = "ThemePark.SessionContext";
        public const string ContextNs = "themepark.SessionContext.com";

        /// <summary>
        /// Gets current UserId or null.
        /// It can be null if no user logged in.
        /// </summary>
        [DataMember]
        public long? UserId { get; set; }

        /// <summary>
        /// Gets current TenantId or null.
        /// This TenantId should be the TenantId of the <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" />.
        /// It can be null if given <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" /> is a host user or no user logged in.
        /// </summary>
        [DataMember]
        public int? TenantId { get; set; }

        /// <summary>
        /// UserId of the impersonator.
        /// This is filled if a user is performing actions behalf of the <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" />.
        /// </summary>
        [DataMember]
        public long? ImpersonatorUserId { get; set; }

        /// <summary>
        /// TenantId of the impersonator.
        /// This is filled if a user with <see cref="P:Abp.Runtime.Session.IAbpSession.ImpersonatorUserId" /> performing actions behalf of the <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" />.
        /// </summary>
        [DataMember]
        public int? ImpersonatorTenantId { get; set; }

        public SessionContext(IAbpSession session)
        {
            UserId = session.UserId;
            TenantId = session.TenantId;
            ImpersonatorUserId = session.ImpersonatorUserId;
            ImpersonatorTenantId = session.ImpersonatorTenantId;
        }
    }
}
