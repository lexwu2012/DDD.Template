using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Reflection;
using System.ServiceModel.Configuration;

namespace ThemePark.Infrastructure.Services
{
    public class ClientFinder
    {
        public IReadOnlyDictionary<Type, ChannelEndpointElement> GetAllClients(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException();
            }

            var serviceGroup = configuration.GetSectionGroup("system.serviceModel") as ServiceModelSectionGroup;
            if (serviceGroup == null)
            {
                throw new ConfigurationErrorsException("No have configuration for system.serviceModel section.");
            }
            var dic = new Dictionary<Type, ChannelEndpointElement>();
            foreach (ChannelEndpointElement endpoint in serviceGroup.Client.Endpoints)
            {
                var type = Type.GetType(endpoint.Contract + "," + 
                    endpoint.Contract.Substring(0, endpoint.Contract.LastIndexOf(".", StringComparison.CurrentCulture)));
                if (type == null)
                {
                    throw new Exception("Failed to get the type by configuration setting.");
                }

                dic.Add(type, endpoint);
            }

            return dic.ToImmutableDictionary();
        }

        public IReadOnlyDictionary<Type, ChannelEndpointElement> GetAllClients()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);

            return GetAllClients(configuration);
        }
    }
}
