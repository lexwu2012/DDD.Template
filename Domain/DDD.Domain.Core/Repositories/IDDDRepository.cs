using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Domain.Repositories;

namespace DDD.Domain.Core.Repositories
{
    public interface IDDDRepository<TEntity, TPrimaryKey> : IRepositoryWithTEntityAndTPrimaryKey<TEntity, TPrimaryKey> 
        where TEntity : class, IAggregateRoot<TPrimaryKey>
    {
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">The entities.</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="entities">The entities.</param>
        void UpdateRange(IEnumerable<TEntity> entities);
    }

    public interface IDDDRepository<TEntity> : IDDDRepository<TEntity, int>, IRepositoryWithEntity<TEntity>
        where TEntity : class, IAggregateRoot<int>
    {

    }
}
