using DDD.Infrastructure.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Web.Threading
{
    public static class InternalAsyncHelper
    {
        public static async Task AwaitTaskWithFinally(Task actualReturnValue, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        /// <summary>
        /// 没有返回值的委托执行
        /// </summary>
        /// <param name="actualReturnValue"></param>
        /// <param name="postAction"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        /// <summary>
        /// 执行AwaitTaskWithPostActionAndFinallyAndGetResult委托
        /// </summary>
        /// <param name="taskReturnType"></param>
        /// <param name="actualReturnValue"></param>
        /// <param name="action"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType, object actualReturnValue, Func<Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                   .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                   .MakeGenericMethod(taskReturnType)
                   .Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }

        /// <summary>
        /// 异步带有返回值的委托执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actualReturnValue"></param>
        /// <param name="postAction"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                var result = await actualReturnValue;
                await postAction();
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                LogHelper.LogException(ex);
                throw;
            }
            finally
            {
                //异步的工作单元的提交（释放资源uow.Dispose）
                finalAction(exception);
            }
        }

        /// <summary>
        /// 没有
        /// </summary>
        /// <param name="taskReturnType"></param>
        /// <param name="actualReturnValue"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static object CallAwaitTaskWithFinallyAndGetResult(Type taskReturnType, object actualReturnValue, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                   .GetMethod("AwaitTaskWithFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                   .MakeGenericMethod(taskReturnType)
                   .Invoke(null, new object[] { actualReturnValue, finalAction });
        }
       
        public static async Task<T> AwaitTaskWithFinallyAndGetResult<T>(Task<T> actualReturnValue, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                return await actualReturnValue;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }
    }
}
