using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Domain.Repositories;

namespace DDD.Domain.Core.Repositories
{
    public static class RepositoryExtension
    {
        public static IQueryable<TEntity> AsNoTracking<TEntity,TPrimaryKey>(this IRepositoryWithTEntityAndTPrimaryKey<TEntity,TPrimaryKey> repository) 
            where TEntity : class, IAggregateRoot<TPrimaryKey>
        {
            var sets = repository.GetAll().As<DbSet<TEntity>>();

            if(sets == null)
                throw new ArgumentException();

            return sets.AsNoTracking();
        }
    }
}
