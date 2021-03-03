using Abp.Dependency;
using Abp.Runtime.Caching;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Security.Serializer;

namespace ThemePark.Infrastructure.Web.Api.Authorization
{
    /// <summary>
    /// Class TokenAuthorizationFilter.
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.IAuthorizationFilter"/>
    /// <seealso cref="Abp.Dependency.ITransientDependency"/>
    public class ApiTokenAuthorizationFilter : IAuthorizationFilter, ITransientDependency
    {
        #region Properties

        /// <summary>
        /// 获取或设置一个值，该值指示是否可以为单个程序元素指定多个已指示特性的实例。
        /// </summary>
        /// <returns>false</returns>
        public bool AllowMultiple { get { return true; } }

        /// <summary>
        /// Gets or sets the cache manager.
        /// </summary>
        /// <value>The cache manager.</value>
        public ICacheManager CacheManager { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 执行要同步的授权筛选器。
        /// </summary>
        /// <returns>要同步的授权筛选器。</returns>
        /// <param name="actionContext">操作上下文。</param>
        /// <param name="cancellationToken">与筛选器关联的取消标记。</param>
        /// <param name="continuation">继续。</param>
        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var auth = actionContext.Request.Headers.Authorization;
                if (auth != null && auth.Scheme == ApiConstant.TokenScheme)
                {
                    var token = await CacheManager.GetTokenCache().GetOrDefaultAsync(auth.Parameter);
                    if (token != null)
                    {
                        try
                        {
                            var ticket = DataSerializers.Ticket.Deserialize(token);
                            actionContext.RequestContext.Principal = new ClaimsPrincipal(ticket.Identity);
                        }
                        catch (Exception)
                        {
                            return actionContext.Request.CreateResponse(HttpStatusCode.OK,
                            Result.FromCode(ResultCode.InvalidToken));
                        }
                    }
                    else
                    {
                        return actionContext.Request.CreateResponse(HttpStatusCode.OK,
                            Result.FromCode(ResultCode.InvalidToken));
                    }
                }
            }

            return await continuation();
        }

        #endregion Methods
    }
}