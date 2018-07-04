using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using DDD.Infrastructure.Ioc;

namespace DDD.Domain.Common.Uow
{
    /// <summary>
    /// UOW拦截注册器
    /// </summary>
    public static class UnitOfWorkRegistrar
    {
        /// <summary>
        /// 初始化拦截器
        /// </summary>
        /// <param name="iocManager"></param>
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                var implementationType = handler.ComponentModel.Implementation.GetTypeInfo();

                RegisterUnitOfWorkAttribute(implementationType, handler);
                RegisterConventionalUnitOfWorkTypes(iocManager, implementationType, handler);
            };
        }

        /// <summary>
        /// 注册UnitOfWorkAttribute拦截器
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="handler"></param>
        private static void RegisterUnitOfWorkAttribute(TypeInfo implementationType, IHandler handler)
        {
            if (IsUnitOfWorkType(implementationType) || AnyMethodHasUnitOfWork(implementationType))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        }

        /// <summary>
        /// 注册通用拦截器（拦截IRepository,IApplicationService）
        /// </summary>
        /// <param name="iocManager"></param>
        /// <param name="implementationType"></param>
        /// <param name="handler"></param>
        private static void RegisterConventionalUnitOfWorkTypes(IIocManager iocManager, TypeInfo implementationType, IHandler handler)
        {
            if (!iocManager.IsRegistered<IUnitOfWorkDefaultOptions>())
            {
                return;
            }

            var uowOptions = iocManager.Resolve<IUnitOfWorkDefaultOptions>();

            if (uowOptions.IsConventionalUowClass(implementationType.AsType()))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        }

        private static bool IsUnitOfWorkType(TypeInfo implementationType)
        {
            return UnitOfWorkHelper.HasUnitOfWorkAttribute(implementationType);
        }

        private static bool AnyMethodHasUnitOfWork(TypeInfo implementationType)
        {
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(UnitOfWorkHelper.HasUnitOfWorkAttribute);
        }
    }
}
