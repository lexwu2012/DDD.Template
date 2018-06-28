using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Entities;
using DDD.Domain.Common.Repositories;
using DDD.Infrastructure.Common.Extensions;

namespace DDD.Domain.Core.Repositories
{
    public static class RepositoryExtension
    {
        public static IQueryable<TEntity> AsNoTracking<TEntity,TPrimaryKey>(this IRepositoryWithTEntityAndTPrimaryKey<TEntity,TPrimaryKey> repository) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var sets = repository.GetAll().As<DbSet<TEntity>>();

            if(sets == null)
                throw new ArgumentException();

            return sets.AsNoTracking();
        }
    }
}
