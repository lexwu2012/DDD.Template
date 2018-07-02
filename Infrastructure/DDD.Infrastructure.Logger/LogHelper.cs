using System;
using Castle.Core.Logging;
using DDD.Infrastructure.Ioc;

namespace DDD.Infrastructure.Logger
{
    public static class LogHelper
    {
        public static ILogger Logger { get; private set; }

        static LogHelper()
        {
            Logger = IocManager.Instance.IsRegistered(typeof(ILoggerFactory))
               ? IocManager.Instance.Resolve<ILoggerFactory>().Create(typeof(LogHelper))
               : NullLogger.Instance;
        }

        public static void LogException(Exception ex)
        {
            LogException(Logger, ex);
        }

        public static void LogException(ILogger logger, Exception ex)
        {
            var severity = (ex as IHasLogSeverity)?.Severity ?? LogSeverity.Error;

            logger.Log(severity, ex.Message, ex);

            LogValidationErrors(logger, ex);
        }


        private static void LogValidationErrors(ILogger logger, Exception exception)
        {

        }
    }
}
