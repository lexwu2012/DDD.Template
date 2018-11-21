using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Castle.Core.Logging;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Web.Application;

namespace DDD.Infrastructure.Web.Mvc.Fliter.Exceptions
{
    public class MvcExceptionFilter: IExceptionFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public MvcExceptionFilter()
        {
            Logger = NullLogger.Instance;
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is TaskCanceledException ||
                filterContext.Exception is OperationCanceledException ||
                filterContext.Exception is ThreadAbortException) return;

            var exception = filterContext.Exception.GetOriginalException();

            //todo: 根据context里面的错误码返回自定义错误
            //var resultCode = GetResultCode(filterContext);

            var exp = filterContext.Exception;
            var builder = new StringBuilder();
            builder.AppendLine(exp.Message);

            exp.GetAllExceptionMessage().ForEach(o => builder.AppendLine(o));

            var message = builder.ToString();
            //var result = Result.FromCode(resultCode, message);

            Logger.Error($"异常消息：{message}\r\n" +
                         $"堆栈信息：\r\n{exception.StackTrace}");            
        }

        private ResultCode GetResultCode(ExceptionContext context)
        {
            return ResultCode.ServerError;
        }
    }
}
