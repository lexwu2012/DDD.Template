using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Domain.CustomAttributes;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Test
{
    [AutoRepositoryTypes(
            typeof(IRepositoryWithEntity<>),
            typeof(IRepositoryWithTEntityAndTPrimaryKey<,>),
            typeof(SampleApplicationEfRepositoryBase<>),
            typeof(SampleApplicationEfRepositoryBase<,>)
            )]
    public class SampleDbContext : DDDDbContext
    {
        public SampleDbContext()
        {

        }

        public SampleDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public SampleDbContext(DbConnection connection)
            : base(connection, false)
        {

        }

    }

    public class SampleApplicationEfRepositoryBase<TEntity> : SampleApplicationEfRepositoryBase<TEntity, int>, IRepositoryWithEntity<TEntity>
          where TEntity : class, IAggregateRoot<int>
    {
        public SampleApplicationEfRepositoryBase(IDbContextProvider<SampleDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    public class SampleApplicationEfRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<SampleDbContext, TEntity, TPrimaryKey>
    where TEntity : class, IAggregateRoot<TPrimaryKey>
    {
        public SampleApplicationEfRepositoryBase(IDbContextProvider<SampleDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
