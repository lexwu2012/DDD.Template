using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using DDD.Infrastructure.Common.Reflection;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Core.DbContextRelate
{
    public class EfDbContextEntityFinder : IDbContextEntityFinder, ITransientDependency
    {
        public IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType)
        {
            return
                from property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                (ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(IDbSet<>)) ||
                 ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>))) &&
                ReflectionHelper.IsAssignableToGenericType(property.PropertyType.GenericTypeArguments[0], typeof(IEntity<>))
                select new EntityTypeInfo(property.PropertyType.GenericTypeArguments[0], property.DeclaringType);
        }
    }
}
