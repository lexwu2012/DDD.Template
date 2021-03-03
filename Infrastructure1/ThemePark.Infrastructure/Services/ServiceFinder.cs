using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Reflection;
using System.ServiceModel.Configuration;

namespace ThemePark.Infrastructure.Services
{
    public class ServiceFinder
    {
        public IReadOnlyDictionary<Type, ServiceElement> GetAllServices(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException();
            }

            var serviceGroup = configuration.GetSectionGroup("system.serviceModel") as ServiceModelSectionGroup;
            if (serviceGroup == null)
            {
                throw new ConfigurationErrorsException("Have no such configuration for system.serviceModel section.");
            }

            var dic = new Dictionary<Type, ServiceElement>();
            foreach (var service in serviceGroup.Services.Services)
            {
                var element = service as ServiceElement;
                if (element == null)
                {
                    throw new ConfigurationErrorsException("Wrong system.serviceModel/services configuration.");
                }

                var type = Type.GetType(element.Name + "," + 
                    element.Name.Substring(0, element.Name.LastIndexOf(".", StringComparison.CurrentCulture)));
                if (type == null)
                {
                    throw new Exception("Failed to get the type by configuration setting.");
                }

                dic.Add(type, element);
            }

            return dic.ToImmutableDictionary();
        }

        public IReadOnlyDictionary<Type, ServiceElement> GetAllServices()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            
            return GetAllServices(configuration);
        }
    }
}
