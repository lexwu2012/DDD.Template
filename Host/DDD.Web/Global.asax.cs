using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using DDD.Infrastructure.AutoMapper.Attributes;
using DDD.Infrastructure.Common.Extensions;

namespace MVCWeb
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //ConfigAllMap()
        }

        private void ConfigAllMap(IMapperConfigurationExpression configuration)
        {
            configuration.ForAllMaps((map, c) =>
            {
                foreach (var property in map.DestinationType.GetProperties())
                {
                    var attr = property.GetAttribute<MapFromAttribute>();
                    if (attr?.PropertyPath?.Length > 0)
                    {
                        c.ForMember(property.Name, m =>
                        {
                            var path = string.Join(".", attr.PropertyPath);
                            var exp = System.Linq.Dynamic.DynamicExpression.ParseLambda(map.SourceType, null, path);

                            var m0 = m.GetType()
                                .GetProperty("PropertyMapActions", BindingFlags.Instance | BindingFlags.NonPublic)?
                                .GetValue(m) as List<Action<PropertyMap>>;
                            var m1 = typeof(PropertyMap).GetMethod(nameof(PropertyMap.MapFrom));
                            m0?.Add(t => m1.Invoke(t, new object[] { exp }));
                        });
                    }
                }
            });
        }
    }
}
