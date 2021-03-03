using System;
using System.Collections.Generic;
using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace ThemePark.Infrastructure
{
    /// <summary>
    /// 全局会话对象
    /// </summary>
    public interface IThemeParkSession
    {
        /// <summary>
        /// Gets current UserId or null.
        /// It can be null if no user logged in.
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// Is empty if no user login or no have permission
        /// </summary>
        List<int> Parks { get; }

        /// <summary>
        /// Gets the agency identifier.
        /// </summary>
        /// <value>The agency identifier.</value>
        int? AgencyId { get; }

        /// <summary>
        /// Gets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        int UserType { get; }

        /// <summary>
        /// Gets the local park code.
        /// </summary>
        /// <value>The local park code.</value>
        int LocalParkCode { get; }

        /// <summary>
        /// Gets the local park identifier.
        /// </summary>
        /// <value>The local park identifier.</value>
        int LocalParkId { get; }

        /// <summary>
        ///指纹机型号  0 ZK 2 TXW
        /// </summary>
        int LocalFingerType { get; }

        /// <summary>
        /// Gets the terminal identifier.
        /// </summary>
        /// <value>The terminal identifier.</value>
        int TerminalId { get; }

        /// <summary>
        /// 设置当前用户为代理商用户类型
        /// </summary>
        /// <param name="agencyId">The agency identifier.</param>
        /// <param name="userType">set userType is AgencyUser</param>
        /// <returns>IDisposable.</returns>
        IDisposable UseAgency(int agencyId, int userType);
    }

    /// <summary>
    /// 空会话
    /// </summary>
    public class NullThemeParkSession : IThemeParkSession, IAbpSession
    {
        /// <summary>
        /// 空会话
        /// </summary>
        public static NullThemeParkSession Instance { get; } = new NullThemeParkSession();

        /// <inheritdoc/>
        private NullThemeParkSession()
        {

        }

        /// <summary>
        /// Used to change <see cref="P:Abp.Runtime.Session.IAbpSession.TenantId" /> and <see cref="P:Abp.Runtime.Session.IAbpSession.UserId" /> for a limited scope.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IDisposable Use(int? tenantId, long? userId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long? UserId => null;

        /// <inheritdoc/>
        public int? AgencyId => null;

        /// <inheritdoc/>
        public List<int> Parks => new List<int>();

        /// <inheritdoc/>
        public int UserType => 0;

        /// <summary>
        /// Gets the local park code.
        /// </summary>
        /// <value>The local park code.</value>
        public int LocalParkCode => 0;

        /// <summary>
        /// Gets the local park identifier.
        /// </summary>
        /// <value>The local park identifier.</value>
        public int LocalParkId => 0;


        /// <summary>
        /// 指纹机型号  0 ZK 2 TXW
        /// </summary>
        public int LocalFingerType => 0;

        /// <summary>
        /// Gets the terminal identifier.
        /// </summary>
        /// <value>The terminal identifier.</value>
        public int TerminalId => 0;

        /// <inheritdoc />
        int? IAbpSession.TenantId => null;

        /// <inheritdoc />
        MultiTenancySides IAbpSession.MultiTenancySide => MultiTenancySides.Host;

        /// <inheritdoc />
        long? IAbpSession.ImpersonatorUserId => null;

        /// <inheritdoc />
        int? IAbpSession.ImpersonatorTenantId => null;

        public IDisposable UseAgency(int agencyId, int userType)
        {
            throw new NotImplementedException();
        }
    }
}
