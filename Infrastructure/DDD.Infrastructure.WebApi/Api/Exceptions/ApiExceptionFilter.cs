using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Castle.Core.Logging;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Domain.Events;
using DDD.Infrastructure.Domain.Events.Exceptions;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Web.Application;

namespace DDD.Infrastructure.WebApi.Api.Exceptions
{
    public class ApiExceptionFilter : ExceptionFilterAttribute, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        public ApiExceptionFilter()
        {
            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is TaskCanceledException ||
                context.Exception is OperationCanceledException ||
                context.Exception is ThreadAbortException) return;

            var exception = context.Exception.GetOriginalException();

            //todo: 根据context里面的错误码返回自定义错误
            var resultCode = GetResultCode(context);

            var exp = context.Exception;
            var builder = new StringBuilder();
            builder.AppendLine(exp.Message);

            exp.GetAllExceptionMessage().ForEach(o => builder.AppendLine(o));

            var message = builder.ToString();
            var result = Result.FromCode(resultCode, message);

            Logger.Error($"请求地址：{context.Request.Method} {context.Request.RequestUri}\r\n" +
                         $"返回提示：{resultCode:D} {resultCode:G}\r\n" +
                         $"异常消息：{message}\r\n" +
                         $"堆栈信息：\r\n{exception.StackTrace}");

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, result);

            //BusinessEvent();

            //触发领域事件，发送邮件
            EventBus.Trigger(this, new HandledExceptionData(context.Exception));
        }

        private ResultCode GetResultCode(HttpActionExecutedContext context)
        {
            return ResultCode.ServerError;
        }

        protected virtual void BusinessEvent()
        {

        }
    }
}
