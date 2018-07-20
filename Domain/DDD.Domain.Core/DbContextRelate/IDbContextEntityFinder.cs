using System;
using System.Collections.Generic;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.DbContextRelate
{
    public interface IDbContextEntityFinder
    {
        IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType);
    }
}
