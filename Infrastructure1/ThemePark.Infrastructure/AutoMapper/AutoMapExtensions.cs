using System;
using AutoMapper;

namespace ThemePark.Infrastructure.AutoMapper
{
    /// <summary>
    /// AutoMap扩展方法
    /// </summary>
    public static class AutoMapExtensions
    {
        /// <summary>
        /// 使用指定的 mapping options 作为映射规则.
        /// </summary>
        /// <typeparam name="TSource">Source type to use</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <param name="opts">Mapping options</param>
        /// <returns>The mapped destination object, same instance as the <paramref name="destination" /> object</returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination,
            Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return Mapper.Map(source, destination, opts);
        }
    }
}