using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Runtime.Caching.Redis;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThemePark.Common;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Cache;
using System.Linq.Dynamic;

namespace ThemePark.Infrastructure
{
    [DependsOn(typeof(AbpAutoMapperModule), typeof(AbpRedisCacheModule))]
    public class InfrastructureModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup. 
        ///             Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
            var mapConfig = Configuration.Modules.AbpAutoMapper().Configurators;

            mapConfig.Add(ConfigAllMap);
            mapConfig.Add(CreateDtoMappings);

            IocManager.Register<IRedisCacheSerializer, RedisCacheSerializer>();
        }

        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 创建 Dto 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        private void CreateDtoMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMissingTypeMaps = true;

            configuration.CreateMap<string, JToken>()
                .ConvertUsing(p => JsonConvert.DeserializeObject<JToken>(p ?? string.Empty));
            configuration.CreateMap<string, JObject>()
                .ConvertUsing(p => JsonConvert.DeserializeObject<JObject>(p ?? string.Empty));
            configuration.CreateMap<string, JArray>()
                .ConvertUsing(p => JsonConvert.DeserializeObject<JArray>(p ?? string.Empty));

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
                            var exp = DynamicExpression.ParseLambda(map.SourceType, null, path);
                            
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