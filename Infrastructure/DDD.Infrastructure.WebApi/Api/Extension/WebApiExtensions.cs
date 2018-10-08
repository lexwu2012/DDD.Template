using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Domain.Uow;

namespace DDD.Infrastructure.WebApi.Api.Extension
{
    public static class WebApiExtensions
    {
        /// <summary>
        /// 获取 Action MethodInfo
        /// </summary>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfoOrNull(this HttpActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ReflectedHttpActionDescriptor)
            {
                return actionDescriptor.As<ReflectedHttpActionDescriptor>().MethodInfo;
            }

            return null;
        }

        /// <summary>
        /// 获取服务对象 
        /// </summary>
        public static TService GetService<TService>(this HttpRequestMessage request)
        {
            return request.GetDependencyScope().GetService<TService>();
        }

        /// <summary>
        /// 获取服务对象 
        /// </summary>
        public static TService GetService<TService>(this IDependencyScope dependencyResolver)
        {
            return (TService)dependencyResolver.GetService(typeof(TService));
        }

        /// <summary>
        /// 创建工作单元选项
        /// </summary>
        internal static UnitOfWorkOptions CreateOptions(this UnitOfWorkAttribute uow)
        {
            return new UnitOfWorkOptions()
            {
                IsTransactional = uow.IsTransactional,
                IsolationLevel = uow.IsolationLevel,
                Timeout = uow.Timeout,
                Scope = uow.Scope
            };
        }
    }
}
