using System;
using System.Linq.Expressions;
using DDD.Infrastructure.Common.Extensions;

namespace DDD.Infrastructure.Web.Query
{
    public class Query<TEntity> : IQuery<TEntity>
       where TEntity : class
    {
        /// <summary>
        /// 指定查询条件
        /// </summary>
        protected Expression<Func<TEntity, bool>> Filter;

        /// <summary>
        /// 创建一个新的 <see cref="Query{TEntity}"/>
        /// </summary>
        public Query()
        {

        }

        /// <summary>
        /// 创建一个指定查询条件的<see cref="Query{TEntity}"/>
        /// </summary>
        /// <param name="filter">指定的查询条件</param>
        public Query(Expression<Func<TEntity, bool>> filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        public virtual Expression<Func<TEntity, bool>> GetFilter()
        {
            return Filter.And(this.GenerateExpression());
        }       
    }
}
