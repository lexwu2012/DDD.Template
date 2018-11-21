using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Castle.Core.Logging;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Web.Mvc.Fliter.Action
{
    public class MvcStatisticsTrackerActionFilter : IActionFilter, ITransientDependency
    {
        private const string StopwatchKey = "StopwatchFilter";
        public ILogger Logger { get; set; }

        public MvcStatisticsTrackerActionFilter()
        {
            Logger = NullLogger.Instance;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items[StopwatchKey] = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var stopwatch = (Stopwatch)filterContext.HttpContext.Items[StopwatchKey];
            stopwatch.Stop();

            var log =
                $"controller:{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}\r\n" +
                $"action:{filterContext.ActionDescriptor.ActionName}\r\n" +
                $"execution time:{stopwatch.ElapsedMilliseconds}ms";

            Logger.Info(log);
        }
    }
}
