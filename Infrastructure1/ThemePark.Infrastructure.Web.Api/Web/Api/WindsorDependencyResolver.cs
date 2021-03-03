using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.Windsor;

namespace ThemePark.Infrastructure.Web.Api
{
    public class WindsorDependencyResolver : IDependencyResolver
    {
        private IWindsorContainer container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// 从范围中检索服务。
        /// </summary>
        /// <returns>
        /// 检索到的服务。
        /// </returns>
        /// <param name="serviceType">要检索的服务。</param>
        public object GetService(Type serviceType)
        {
            if (!container.Kernel.HasComponent(serviceType)) return null;
            return container.Resolve(serviceType);
        }

        /// <summary>
        /// 从范围中检索服务集合。
        /// </summary>
        /// <returns>
        /// 检索到的服务集合。
        /// </returns>
        /// <param name="serviceType">要检索的服务集合。</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.ResolveAll(serviceType).Cast<object>();
        }

        /// <summary>
        /// 开始解析范围。
        /// </summary>
        /// <returns>
        /// 依赖范围。
        /// </returns>
        public IDependencyScope BeginScope()
        {
            var childContainer = new WindsorContainer();
            container.AddChildContainer(childContainer);

            return new WindsorDependencyResolver(childContainer);
        }

        public void Dispose()
        {
            if (container.Parent != null)
            {
                container.RemoveChildContainer(container);
            }
            container.Dispose();
        }
    }
}
