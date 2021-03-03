using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;

namespace ThemePark.Infrastructure.EntityFramework
{
    public static class RepositoryExtension
    {
        /// <summary>
        /// 返回不附加在<see cref="T:System.Data.Entity.DbContext" />上的查询
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> AsNoTracking<TEntity>(this IRepository<TEntity> repository)
            where TEntity : class, IEntity<int>
        {
            return AsNoTracking<TEntity, int>(repository);
        }

        /// <summary>
        /// 返回不附加在<see cref="T:System.Data.Entity.DbContext" />上的查询
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TPrimaryKey">实体主键类型</typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> AsNoTracking<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var set = repository.GetAll().As<DbSet<TEntity>>();
            if (set == null)
            {
                throw new ArgumentException();
            }

            return set.AsNoTracking();
        }

        /// <summary>
        ///Used to get a IQueryable that is used to retrieve entities from entire table.
        /// One or more
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TPrimaryKey">实体主键类型</typeparam>
        /// <param name="repository"></param>
        /// <param name="propertySelectors">A list of include expressions.</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public static IQueryable<TEntity> AsNoTrackingAndInclude<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            params Expression<Func<TEntity, object>>[] propertySelectors)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var query = repository.AsNoTracking();

            foreach (var selector in propertySelectors)
            {
                query = query.Include(selector);
            }

            return query;
        }

        /// <summary>
        ///Used to get a IQueryable that is used to retrieve entities from entire table.
        /// One or more
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="repository"></param>
        /// <param name="propertySelectors">A list of include expressions.</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public static IQueryable<TEntity> AsNoTrackingAndInclude<TEntity>(
            this IRepository<TEntity> repository, params Expression<Func<TEntity, object>>[] propertySelectors)
            where TEntity : class, IEntity<int>
        {
            return AsNoTrackingAndInclude<TEntity, int>(repository, propertySelectors);
        }
    }
}
