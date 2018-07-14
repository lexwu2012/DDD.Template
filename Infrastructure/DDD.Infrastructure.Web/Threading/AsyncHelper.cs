﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Web.Threading
{
    public static class AsyncHelper
    {
        /// <summary>
        /// Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsync(this MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            );
        }

        /// <summary>
        /// Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        [Obsolete("Use MethodInfo.IsAsync() extension method!")]
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return method.IsAsync();
        }

        ///// <summary>
        ///// Runs a async method synchronously.
        ///// </summary>
        ///// <param name="func">A function that returns a result</param>
        ///// <typeparam name="TResult">Result type</typeparam>
        ///// <returns>Result of the async operation</returns>
        //public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        //{
        //    return AsyncContext.Run(func);
        //}

        ///// <summary>
        ///// Runs a async method synchronously.
        ///// </summary>
        ///// <param name="action">An async action</param>
        //public static void RunSync(Func<Task> action)
        //{
        //    AsyncContext.Run(action);
        //}
    }
}
