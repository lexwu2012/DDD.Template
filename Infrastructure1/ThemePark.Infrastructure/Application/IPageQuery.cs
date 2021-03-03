using System;
using System.Linq.Expressions;
using ThemePark.Common;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 定义分页查询
    /// </summary>
    /// <typeparam name="TEntity">要查询的实体类型</typeparam>
    public interface IPageQuery<TEntity> : IPageSortInfo, IQuery<TEntity>
        where TEntity : class
    {

    }

    /// <summary>
    /// 分页查询
    /// </summary>
    public class PageQuery<TEntity> : PageSortInfo, IPageQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 指定查询条件
        /// </summary>
        protected Expression<Func<TEntity, bool>> Filter;

        /// <summary>
        /// 创建一个新的 <see cref="Query{TEntity}"/>
        /// </summary>
        public PageQuery()
        {

        }

        /// <summary>
        /// 创建一个指定查询条件的<see cref="Query{TEntity}"/>
        /// </summary>
        /// <param name="filter">指定的查询条件</param>
        public PageQuery(Expression<Func<TEntity, bool>> filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        public virtual Expression<Func<TEntity, bool>> GetFilter()
        {
            return Filter.And(this.GetQueryExpression());
        }
    }
}