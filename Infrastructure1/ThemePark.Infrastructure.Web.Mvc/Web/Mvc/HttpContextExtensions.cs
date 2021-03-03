using System.Web;
using ThemePark.Infrastructure.Security;

namespace ThemePark.Infrastructure.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 设置 Cookie 验证授权
        /// </summary>
        public static void SetCookieTicket(this HttpContextBase request, AuthenticationTicket ticket)
        {
            if (ticket == null)
                request.Items.Remove(MvcConstant.CookieAuth);
            else
                request.Items[MvcConstant.CookieAuth] = ticket;
        }

        /// <summary>
        /// 获取 Cookie 验证授权
        /// </summary>
        public static AuthenticationTicket GetCookieTicket(this HttpContextBase request)
        {
            return request.Items.Contains(MvcConstant.CookieAuth)
                ? request.Items[MvcConstant.CookieAuth] as AuthenticationTicket
                : null;
        }
    }
}