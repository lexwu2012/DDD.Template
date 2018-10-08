using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.WebApi.Api.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DDD.Infrastructure.WebApi.Uow
{
    public class ApiUowFilter : IActionFilter
    {
        public bool AllowMultiple => true;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ApiUowFilter(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, 
            Func<Task<HttpResponseMessage>> continuation)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();

            if (null == methodInfo)
                return await continuation();

            var uowAttribute = methodInfo.GetAttribute<UnitOfWorkAttribute>() ?? new UnitOfWorkAttribute();

            if(uowAttribute.IsDisabled)
                return await continuation();

            using (var uow = _unitOfWorkManager.Begin(uowAttribute.CreateOptions()))
            {
                var result = await continuation();
                await uow.CompleteAsync();
                return result;
            }
        }
    }
}
