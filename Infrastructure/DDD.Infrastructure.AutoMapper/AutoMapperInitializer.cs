using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DDD.Infrastructure.AutoMapper.Attributes;
using DDD.Infrastructure.AutoMapper.Extension;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Common.Reflection;

namespace DDD.Infrastructure.AutoMapper
{
    public class AutoMapperInitializer : IAutoMapperInitializer
    {
        private readonly ITypeFinder _typeFinder;

        public AutoMapperInitializer(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public void Initial()
        {

            var list = new List<Action<IMapperConfigurationExpression>>();
            list.Add(ConfigAllMap);

            Action<IMapperConfigurationExpression> action = mapperConfigurationExpression =>
            {
                FindAndAutoMapTypes(mapperConfigurationExpression);
                foreach (var configurator in list)
                {
                    configurator(mapperConfigurationExpression);
                }
            };

            Mapper.Initialize(action);
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {

            var types = _typeFinder.Find(type =>
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                       typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                       typeInfo.IsDefined(typeof(AutoMapToAttribute));
            }
            );
            foreach (var type in types)
            {
                configuration.CreateAutoAttributeMaps(type);
            }
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
