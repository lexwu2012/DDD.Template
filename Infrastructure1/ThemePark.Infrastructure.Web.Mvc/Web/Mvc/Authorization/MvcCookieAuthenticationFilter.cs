using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Abp.Dependency;
using ThemePark.Infrastructure.Security.DataProtection;
using ThemePark.Infrastructure.Security.Serializer;
using ThemePark.Infrastructure.Security.Serializer.Encoder;

namespace ThemePark.Infrastructure.Web.Mvc.Authorization
{
    /// <inheritdoc/>
    public class MvcCookieAuthenticationFilter : FilterAttribute, IAuthenticationFilter, ITransientDependency, IActionFilter
    {
        private IDataProtectionProvider _dataProtectionProvider;

        private readonly string _cookieName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcCookieAuthenticationFilter" /> class.
        /// </summary>
        /// <param name="dataProtectionProvider">The data protection provider.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        public MvcCookieAuthenticationFilter(IDataProtectionProvider dataProtectionProvider, string cookieName)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _cookieName = cookieName;
        }

        /// <summary>
        /// 对请求进行身份验证。
        /// </summary>
        /// <param name="filterContext">用于身份验证的上下文。</param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var cookie = filterContext.HttpContext.Request.Cookies.Get(_cookieName);

                if (cookie != null)
                {
                    try
                    {
                        byte[] data;
                        var bytes = TextEncodings.Base64Url.Decode(cookie.Value);
                        if (bytes.Length > 0 && _dataProtectionProvider.Create(MvcConstant.CookieAuth).TryUnprotect(bytes, out data))
                        {
                            var ticket = DataSerializers.Ticket.Deserialize(data);
                            filterContext.Principal = new ClaimsPrincipal(ticket.Identity);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 向当前 <see cref="T:System.Web.Mvc.ActionResult"/> 添加身份验证质询。
        /// </summary>
        /// <param name="filterContext">用于身份验证质询的上下文。</param>
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }

        /// <summary>
        /// 在执行操作方法之前调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        /// <summary>
        /// 在执行操作方法后调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var ticket = filterContext.HttpContext.GetCookieTicket();
            if (ticket != null)
            {
                string value;
                if (ticket.Properties.ExpiresUtc < DateTimeOffset.UtcNow)
                    value = string.Empty;
                else
                {
                    var data = DataSerializers.Ticket.Serialize(ticket);
                    value = TextEncodings.Base64Url.Encode(_dataProtectionProvider.Create(
                        MvcConstant.CookieAuth).Protect(data));
                }
                var cookie = new HttpCookie(_cookieName, value)
                {
                    Domain = ticket.Properties.Domain,
                    HttpOnly = true,
                    Path = "/"
                };

                var expires = ticket.Properties.ExpiresUtc;
                if (expires.HasValue)
                    cookie.Expires = expires.Value.UtcDateTime;

                filterContext.HttpContext.Response.SetCookie(cookie);
            }
        }
    }
}
