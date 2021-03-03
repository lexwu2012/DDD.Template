using System;
using System.Threading;

namespace ThemePark.Common
{
    public static class Work
    {
        /// <summary>
        /// Retries the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="errorHandler">The error handler.</param>
        /// <param name="failHandler">The fail handler.</param>
        /// <param name="times">The times.</param>
        /// <param name="during">The during.</param>
        public static void Retry(Action action, Action<Exception> errorHandler, Action failHandler, int times = 1, int during = 1000)
        {
            bool hasExce = false;

            do
            {
                try
                {
                    action();
                    hasExce = false;
                }
                catch (Exception ex)
                {
                    hasExce = true;
                    errorHandler?.Invoke(ex);
                    Thread.Sleep(during);
                }
            } while (hasExce && times-- > 0);

            if (hasExce)
            {
                failHandler?.Invoke();
            }
        }

        /// <summary>
        /// Retries the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="errorHandler">The error handler.</param>
        /// <param name="failHandler">The fail handler.</param>
        /// <param name="times">The times.</param>
        /// <param name="during">The during.</param>
        /// <returns>T.</returns>
        public static T Retry<T>(Func<T> action, Action<Exception> errorHandler, Action<Exception> failHandler, int times = 1, int during = 1000)
        {
            bool hasExce = false;
            T result = default(T);
            Exception lastException = null;

            do
            {
                try
                {
                    result = action();
                    hasExce = false;
                }
                catch (Exception ex)
                {
                    hasExce = true;
                    lastException = ex;
                    errorHandler?.Invoke(ex);
                    Thread.Sleep(during);
                }
            } while (hasExce && times-- > 0);

            if (hasExce)
            {
                failHandler?.Invoke(lastException);
            }

            return result;
        }
    }
}
