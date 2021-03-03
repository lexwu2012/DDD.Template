using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Runtime.Validation;
using ThemePark.Common;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Infrastructure.Web.Api.Validation
{
    public class AbpApiValidationFilter : IActionFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IIocResolver _iocResolver;

        public AbpApiValidationFilter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

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