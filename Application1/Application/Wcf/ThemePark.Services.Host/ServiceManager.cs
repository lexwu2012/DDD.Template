using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using ThemePark.Infrastructure.Services;

namespace ThemePark.Services.Host
{
    public class ServiceManager : IServiceManager
    {
        private readonly Dictionary<Type, ServiceHostBase> _allServiceHosts;

        public Dictionary<Type, ServiceHostBase> AllServiceHosts => _allServiceHosts;

        public ServiceManager()
        {
            _allServiceHosts = new Dictionary<Type, ServiceHostBase>();
        }

        /// <summary>
        /// start all services
        /// </summary>
        public void StartAllServices()
        {
            foreach (var serviceHost in _allServiceHosts)
            {
                serviceHost.Value.Open();
            }
        }

        /// <summary>
        /// stop all services
        /// </summary>
        public void StopAllServices()
        {
            foreach (var serviceHost in _allServiceHosts)
            {
                serviceHost.Value.Close();
            }
        }

        /// <summary>
        ///   Implementors should perform any initialization logic.
        /// </summary>
        public void Initialize()
        {
            var container = ServiceContext.Current.IocContainer;
            var services = container.Resolve<ServiceFinder>().GetAllServices();

            var type = typeof(IWcfService);
            var dic = services.Where(o => type.IsAssignableFrom(o.Key)).ToList();

            if (container.Kernel.GetFacilities().All(o => o.GetType() != typeof(WcfFacility)))
            {
                container.AddFacility<WcfFacility>();
            }
            //register all wcf services
            foreach (var service in dic)
            {
                var serviceType = service.Key;
                var interfaceType = serviceType.GetInterface("I" + serviceType.Name);
                container.Register(Component.For(interfaceType).ImplementedBy(serviceType).LifestylePerWcfSession());
            }

            var factory = new DefaultServiceHostFactory(container.Kernel);

            //create ServiceHost for services
            foreach (var service in dic)
            {
                var host = factory.CreateServiceHost(service.Key.FullName, new Uri[] { });
                host.Opened +=
                    (sender, args) => Console.WriteLine(((ServiceHost) sender).Description.Name + 
                        " opened. status: " + ((ServiceHost) sender).State);
                _allServiceHosts.Add(service.Key, host);
            }
        }
    }
}
