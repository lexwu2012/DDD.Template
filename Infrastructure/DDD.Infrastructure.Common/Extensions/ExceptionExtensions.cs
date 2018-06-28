using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace DDD.Infrastructure.Common.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 获取异常的原始异常信息(<see cref="System.Exception.InnerException"/>)
        /// </summary>
        public static Exception GetOriginalException(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            while (exception.InnerException != null)
                exception = exception.InnerException;

            return exception;
        }

        /// <summary>
        /// 使用 <see cref="ExceptionDispatchInfo.Capture"/> 方法重抛异常
        /// 保留堆栈信息
        /// </summary>
        public static void ReThrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        /// <summary>
        /// 返回所有异常的描述信息
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>System.String[].</returns>
        public static string[] GetAllExceptionMessage(this Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            List<string> list = new List<string>();
            do
            {
                list.Add(exception.Message);
                exception = exception.InnerException;
            } while (exception != null);

            return list.ToArray();
        }
    }
}
