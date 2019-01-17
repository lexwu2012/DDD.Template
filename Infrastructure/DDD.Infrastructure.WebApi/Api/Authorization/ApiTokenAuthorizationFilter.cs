using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Threading;
using System.Web.Http.Controllers;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Web.Application;
using DDD.Infrastructure.WebApi.Api.Helper;
using DDD.Infrastructure.Logger;

namespace DDD.Infrastructure.WebApi.Api.Authorization
{
    public class ApiTokenAuthorizationFilter : IAuthorizationFilter, ITransientDependency
    {
        public bool AllowMultiple => true;


        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var auth = actionContext.Request.Headers.Authorization;
                if (auth != null && auth.Scheme == "Bearer")
                {
                    var token = auth.Parameter;

                    //todo: get the token from the cache                   
                    if (token != null)
                    {
                        try
                        {
                            actionContext.RequestContext.Principal = TokenHelper.GetPrincipal(token);
                        }
                        catch (Exception exception)
                        {
                            LogHelper.LogException(exception);
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
    }
}
