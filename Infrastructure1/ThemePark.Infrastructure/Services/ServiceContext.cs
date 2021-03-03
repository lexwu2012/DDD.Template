using System;
using Abp;
using Abp.Dependency;
using Castle.Windsor;

namespace ThemePark.Infrastructure.Services
{
    public class ServiceContext : IShouldInitialize
    {
        private static ServiceContext _singletone;

        public IServiceManager ServiceManager { get; }

        /// <summary>Reference to the Castle Windsor Container.</summary>
        public IWindsorContainer IocContainer { get; private set; }

        public ServiceContext(IServiceManager serviceManager)
        {
            if (_singletone == null)
            {
                _singletone = this;
                ServiceManager = serviceManager;
                IocContainer = IocManager.Instance.IocContainer;
            }
        }

        /// <summary>
        ///   Implementors should perform any initialization logic.
        /// </summary>
        public void Initialize()
        {
            ServiceManager.Initialize();
        }

        public static ServiceContext Current
        {
            get
            {
                if (_singletone == null)
                {
                    throw new Exception("Have no such a ServiceContext instance.");
                }
                return _singletone;
            }
        }
    }
}
