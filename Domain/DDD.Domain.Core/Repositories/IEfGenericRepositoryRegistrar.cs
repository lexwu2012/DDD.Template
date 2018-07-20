using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.CustomAttributes;
using DDD.Infrastructure.Ioc;

namespace DDD.Domain.Core.Repositories
{
    public interface IEfGenericRepositoryRegistrar
    {
        void RegisterForDbContext(Type dbContextType, IIocManager iocManager, AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute);
    }
}
