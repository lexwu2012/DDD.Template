using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Dependency;
using ThemePark.Infrastructure.Security.DataProtection;
using ThemePark.Infrastructure.Security.Serializer;
using ThemePark.Infrastructure.Security.Serializer.Encoder;

namespace ThemePark.Infrastructure.Web.Api.Authorization
{
    public class ApiCookieAuthorizationFilter : IAuthorizationFilter, ITransientDependency, IActionFilter
    {
        private IDataProtectionProvider _dataProtectionProvider;

        private readonly string _cookieName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCookieAuthorizationFilter" /> class.
        /// </summary>
        /// <param name="dataProtectionProvider">The data protection provider.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        public ApiCookieAuthorizationFilter(IDataProtectionProvider dataProtectionProvider, string cookieName)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _cookieName = cookieName;
        }

        /// <summary>获取或设置一个值，该值指示是否可以为单个程序元素指定多个已指示特性的实例。</summary>
        /// <returns>如果可以指定多个实例，则为 true；否则为 false。默认值为 false。</returns>
        public bool AllowMultiple { get { return false; } }

        /// <summary>异步执行筛选器操作。</summary>
        /// <returns>为此操作新建的任务。</returns>
        /// <param name="actionContext">操作上下文。</param>
        /// <param name="cancellationToken">为此任务分配的取消标记。</param>
        /// <param name="continuation">在调用操作方法之后，委托函数将继续。</param>
        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var response = await continuation();

            var ticket = actionContext.Request.GetCookieTicket();
            if (ticket != null)
            {
                string value;
                if (ticket.Properties.ExpiresUtc < DateTimeOffset.UtcNow)
                    value = string.Empty;
                else
                {
                    var data = DataSerializers.Ticket.Serialize(ticket);
                    value = TextEncodings.Base64Url.Encode(_dataProtectionProvider.Create(
                        ApiConstant.CookieAuth).Protect(data));
                }

                var cookie = new CookieHeaderValue(_cookieName, value)
                {
                    Domain = ticket.Properties.Domain,
                    Expires = ticket.Properties.ExpiresUtc,
                    HttpOnly = true,
                    Path = "/"
                };

                response.Headers.AddCookies(new[] { cookie });
            }


            return response;
        }

        /// <summary>
        /// 执行要同步的授权筛选器。
        /// </summary>
        /// <param name="actionContext">操作上下文。</param>
        /// <param name="cancellationToken">与筛选器关联的取消标记。</param>
        /// <param name="continuation">继续。</param>
        /// <returns>要同步的授权筛选器。</returns>
        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var cookie = actionContext.Request.Headers.GetCookies(_cookieName).FirstOrDefault();

                if (cookie != null)
                {
                    try
                    {
                        byte[] data;
                        var bytes = TextEncodings.Base64Url.Decode(cookie[_cookieName].Value);
                        if (bytes.Length > 0 && _dataProtectionProvider.Create(ApiConstant.CookieAuth).TryUnprotect(bytes, out data))
                        {
                            var ticket = DataSerializers.Ticket.Deserialize(data);
                            actionContext.RequestContext.Principal = new ClaimsPrincipal(ticket.Identity);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return await continuation();
        }
    }
}
