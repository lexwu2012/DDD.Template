using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.CustomAttributes;
using DDD.Domain.Common.Repositories;

namespace DDD.Domain.Core.Repositories
{
    public static class EfAutoRepositoryTypes
    {
        public static AutoRepositoryTypesAttribute Default { get; private set; }

        static EfAutoRepositoryTypes()
        {
            Default = new AutoRepositoryTypesAttribute(
                typeof(IRepositoryWithEntity<>),
                typeof(IRepositoryWithTEntityAndTPrimaryKey<,>),
                typeof(EfRepositoryBase<,>),
                typeof(EfRepositoryBase<,,>)
            );
        }
    }
}
