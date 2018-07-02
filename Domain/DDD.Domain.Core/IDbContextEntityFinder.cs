using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.BaseEntities;

namespace DDD.Domain.Core
{
    public interface IDbContextEntityFinder
    {
        IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType);
    }
}
