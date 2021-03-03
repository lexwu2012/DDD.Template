using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Runtime.Session;
using Abp.Runtime.Validation;
using Castle.Core.Logging;
using ThemePark.Common;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Infrastructure.Web.Api.Exceptions
{
    public class ApiExceptionFilter : ExceptionFilterAttribute, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IAbpSession AbpSession { get; set; }

        public IEventBus EventBus { get; set; }

        public ApiExceptionFilter()
        {
            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is TaskCanceledException ||
                context.Exception is OperationCanceledException ||
                context.Exception is ThreadAbortException) return;

            var exception = context.Exception.GetOriginalException();
            var resultCode = GetResultCode(context);

            var exp = context.Exception;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(exp.Message);

            var validExp = exp as AbpValidationException;
            if (validExp != null)
            {
                validExp.ValidationErrors.ForEach(o => builder.Append(o.ErrorMessage + "\t"));
            }
            else
            {
                exp.GetAllExceptionMessage().ForEach(o => builder.AppendLine(o));
            }

            var message = builder.ToString();
            var result = Result.FromCode(resultCode, message);

            Logger.Error($"请求地址：{context.Request.Method} {context.Request.RequestUri}\r\n" +
                         $"返回提示：{resultCode:D} {resultCode:G}\r\n" +
                         $"异常消息：{message}\r\n" +
                         $"堆栈信息：\r\n{exception.StackTrace}");

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, result);

            EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));
        }

        private ResultCode GetResultCode(HttpActionExecutedContext context)
        {
            if (context.Exception is Abp.Authorization.AbpAuthorizationException)
            {
                return AbpSession.UserId.HasValue
                    ? ResultCode.Forbidden
                    : ResultCode.Unauthorized;
            }

            return ResultCode.ServerError;
        }
    }
}
