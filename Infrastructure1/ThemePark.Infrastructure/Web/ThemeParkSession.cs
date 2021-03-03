using System;
using System.Linq;
using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using System.Collections.Generic;
using System.Configuration;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Castle.Core;

namespace ThemePark.Infrastructure.Web
{
    /// <summary>
    /// 全局会话对象
    /// </summary>
    [CastleComponent(nameof(ThemePark) + nameof(ThemePark.Infrastructure) + nameof(ThemePark.Infrastructure.IThemeParkSession))]
    public class ThemeParkSession : ClaimsAbpSession, IThemeParkSession, ISingletonDependency
    {
        /// <summary>
        /// Is empty if no user login or no have permission
        /// </summary>
        public List<int> Parks
        {
            get
            {
                var areaClaims = PrincipalAccessor.Principal?.Claims
                    .Where(o => o.Type.Equals(ThemeParkClaimTypes.ParkArea))
                    .ToList();

                if (areaClaims == null || !areaClaims.Any())
                {
                    return new List<int>();
                }

                if (areaClaims.Any(o => string.IsNullOrWhiteSpace(o.Value)))
                {

                }

                return areaClaims.Where(o => !string.IsNullOrWhiteSpace(o.Value))
                    .Select(o => int.Parse(o.Value)).ToList();
            }
        }

        /// <summary>
        /// Gets the agency identifier.
        /// </summary>
        /// <value>The agency identifier.</value>
        public int? AgencyId
        {
            get
            {
                if (SessionOverride != null)
                {
                    return SessionOverride.AgencyId;
                }

                var claim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(o => o.Type.Equals(ThemeParkClaimTypes.Agency));
                if (string.IsNullOrEmpty(claim?.Value))
                {
                    return null;
                }

                int id;
                if (int.TryParse(claim.Value, out id))
                {
                    return id;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        public int UserType
        {
            get
            {
                if (SessionOverride != null)
                {
                    return SessionOverride.UserType;
                }

                var claim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(o => o.Type.Equals(ThemeParkClaimTypes.UserType));
                if (string.IsNullOrWhiteSpace(claim?.Value))
                {
                    return 0;
                }

                int type;
                if (int.TryParse(claim.Value, out type))
                {
                    return type;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the local park code.
        /// </summary>
        /// <value>The local park code.</value>
        public int LocalParkCode
        {
            get
            {
                var local = ConfigurationManager.AppSettings["LocalParkCode"];
                return int.Parse(local);
            }
        }

        /// <summary>
        /// Gets the local park identifier.
        /// </summary>
        /// <value>The local park identifier.</value>
        public int LocalParkId
        {
            get
            {
                var local = ConfigurationManager.AppSettings["LocalParkId"];
                return int.Parse(local);
            }
        }

        /// <summary>
        /// 指纹机类型
        /// </summary>
        public int LocalFingerType
        {
            get
            {
                var local = ConfigurationManager.AppSettings["LocalFingerType"];
                return int.Parse(local);
            }
        }

        /// <summary>
        /// Gets the terminal identifier.
        /// </summary>
        /// <value>The terminal identifier.</value>
        public int TerminalId
        {
            get
            {
                var claim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(o => o.Type.Equals(ThemeParkClaimTypes.Terminal));
                if (string.IsNullOrWhiteSpace(claim?.Value))
                {
                    return 0;
                }

                int type;
                if (int.TryParse(claim.Value, out type))
                {
                    return type;
                }

                return 0;
            }
        }

        /// <summary>
        /// The override scope key
        /// </summary>
        private const string OverrideScopeKey = "ThemeSession.Override";

        private ThemeParkSessionOverride SessionOverride => ThemeParkSessionOverrideScopeProvider.GetValue(OverrideScopeKey);

        /// <summary>
        /// Gets the theme park session override scope provider.
        /// </summary>
        /// <value>The theme park session override scope provider.</value>
        protected IAmbientScopeProvider<ThemeParkSessionOverride> ThemeParkSessionOverrideScopeProvider { get; }

        public ThemeParkSession(IPrincipalAccessor principalAccessor,
            IMultiTenancyConfig multiTenancy,
            ITenantResolver tenantResolver,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider,
            IAmbientScopeProvider<ThemeParkSessionOverride> themeParkSessionOverrideScopeProvider) : base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
        {
            ThemeParkSessionOverrideScopeProvider = themeParkSessionOverrideScopeProvider;
        }

        /// <summary>
        /// 设置当前用户为代理商用户类型
        /// </summary>
        /// <param name="agencyId">The agency identifier.</param>
        /// <param name="userType">set userType is AgencyUser</param>
        /// <returns>IDisposable.</returns>
        public IDisposable UseAgency(int agencyId, int userType)
        {
            return ThemeParkSessionOverrideScopeProvider.BeginScope(OverrideScopeKey, new ThemeParkSessionOverride(agencyId, userType));
        }
    }
}
