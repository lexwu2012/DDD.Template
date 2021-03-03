using System.Net.Http;
using ThemePark.Infrastructure.Security;

namespace ThemePark.Infrastructure.Web.Api
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// 设置 Cookie 验证授权
        /// </summary>
        public static void SetCookieTicket(this HttpRequestMessage request, AuthenticationTicket ticket)
        {
            if (ticket == null)
                request.Properties.Remove(ApiConstant.CookieAuth);
            else
                request.Properties[ApiConstant.CookieAuth] = ticket;
        }


        /// <summary>
        /// 获取 Cookie 验证授权
        /// </summary>
        public static AuthenticationTicket GetCookieTicket(this HttpRequestMessage request)
        {
            object obj;
            return request.Properties.TryGetValue(ApiConstant.CookieAuth, out obj)
                ? obj as AuthenticationTicket
                : null;
        }
    }
}
