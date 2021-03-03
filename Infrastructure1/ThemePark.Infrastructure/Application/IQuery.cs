using System;
using System.Linq.Expressions;
using ThemePark.Common;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 定义查询参数
    /// </summary>
    /// <typeparam name="TEntity">要查询的实体类型</typeparam>
    public interface IQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 获取查询条件
        /// </summary>
        Expression<Func<TEntity, bool>> GetFilter();
    }


    /// <summary>
    /// 定义查询参数
    /// </summary>
    /// <typeparam name="TEntity">要查询的实体类型</typeparam>
    public class Query<TEntity> : IQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 指定查询条件
        /// </summary>
        protected Expression<Func<TEntity, bool>> _filter;

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
            _filter = filter;
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        public virtual Expression<Func<TEntity, bool>> GetFilter()
        {
            return _filter.And(this.GetQueryExpression());
        }
    }
}
