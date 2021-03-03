using System.Security.Claims;

namespace ThemePark.Infrastructure.Security
{
    /// <summary>
    /// 身份验证凭据
    /// </summary>
    public class AuthenticationTicket
    {
        /// <summary>
        /// 身份标识
        /// </summary>
        public ClaimsIdentity Identity { get; }


        /// <summary>
        /// 身份凭据数据
        /// </summary>
        public AuthenticationProperties Properties { get; }


        /// <summary>
        /// 身份验证凭据
        /// </summary>
        public AuthenticationTicket(ClaimsIdentity identity, AuthenticationProperties properties = null)
        {
            Identity = identity ?? new ClaimsIdentity();
            Properties = properties ?? new AuthenticationProperties();
        }
    }
}
