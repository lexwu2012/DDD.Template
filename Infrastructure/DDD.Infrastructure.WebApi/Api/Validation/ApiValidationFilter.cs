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
    /// <summary>
    /// api参数验证过滤器
    /// </summary>
    public class ApiValidationFilter : IActionFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IIocResolver _iocResolver;

        //public ApiValidationFilter(IIocResolver iocResolver)
        //{
        //    _iocResolver = iocResolver;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
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

            //有参数没通过验证
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
