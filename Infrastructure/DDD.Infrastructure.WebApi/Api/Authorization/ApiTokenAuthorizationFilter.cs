using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Threading;
using System.Web.Http.Controllers;

namespace DDD.Infrastructure.WebApi.Api.Authorization
{
    public class ApiTokenAuthorizationFilter : IAuthorizationFilter
    {
        public bool AllowMultiple => true;
        

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            
            return await continuation();
        }
    }
}
