using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Web.Application;
using DDD.Infrastructure.Web.CustomAttributes;
using DDD.Infrastructure.WebApi.Api.Extension;

namespace DDD.Infrastructure.WebApi.Api.Validation
{
    public class ApiValidationFilter : IActionFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IIocResolver _iocResolver;

        //public ApiValidationFilter(IIocResolver iocResolver)
        //{
        //    _iocResolver = iocResolver;
        //}
        

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var method = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (method == null)
            {
                return await continuation();
            }

            if (IsValidationDisabled(method))
            {
                return await continuation();
            }

            if (!actionContext.ModelState.IsValid)
            {
                var error = actionContext.ModelState.GetValidationSummary();
                var result = Result.FromError($"参数验证不通过：{error}", ResultCode.InvalidData);
                return actionContext.Request.CreateResponse(result);
            }

            return await continuation();
        }

        protected virtual bool IsValidationDisabled(MethodInfo method)
        {
            if (method.IsDefined(typeof(EnableValidationAttribute), true))
            {
                return false;
            }

            return method.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableValidationAttribute>() != null;
        }
    }
}
