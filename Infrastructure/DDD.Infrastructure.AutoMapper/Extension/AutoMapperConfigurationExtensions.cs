using System;
using System.Reflection;
using AutoMapper;
using DDD.Infrastructure.AutoMapper.Attributes;

namespace DDD.Infrastructure.AutoMapper.Extension
{
    public static class AutoMapperConfigurationExtensions
    {
        public static void CreateAutoAttributeMaps(this IMapperConfigurationExpression configuration, Type type)
        {
            foreach (var autoMapAttribute in type.GetTypeInfo().GetCustomAttributes<AutoMapAttributeBase>())
            {
                autoMapAttribute.CreateMap(configuration, type);
            }
        }
    }
}
