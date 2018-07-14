using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.BaseEntities;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Entities;
using DDD.Infrastructure.Common.Reflection;

namespace DDD.Domain.Core.Repositories
{
    public static class EfRepositoryExtensions
    {
        public static DbContext GetDbContext<TEntity, TPrimaryKey>(this IRepositoryWithTEntityAndTPrimaryKey<TEntity, TPrimaryKey> repository)
            where TEntity : class, IAggregateRoot<TPrimaryKey>
        {
            var repositoryWithDbContext = ProxyHelper.UnProxy(repository) as IRepositoryWithDbContext;
            if (repositoryWithDbContext != null)
            {
                return repositoryWithDbContext.GetDbContext();
            }

            throw new ArgumentException("Given repository does not implement IRepositoryWithDbContext", nameof(repository));
        }

        public static void DetachFromDbContext<TEntity, TPrimaryKey>(this IRepositoryWithTEntityAndTPrimaryKey<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IAggregateRoot<TPrimaryKey>
        {
            repository.GetDbContext().Entry(entity).State = EntityState.Detached;
        }
    }
}
