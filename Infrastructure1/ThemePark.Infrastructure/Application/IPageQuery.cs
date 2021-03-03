using System;
using System.Linq.Expressions;
using ThemePark.Common;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// �����ҳ��ѯ
    /// </summary>
    /// <typeparam name="TEntity">Ҫ��ѯ��ʵ������</typeparam>
    public interface IPageQuery<TEntity> : IPageSortInfo, IQuery<TEntity>
        where TEntity : class
    {

    }

    /// <summary>
    /// ��ҳ��ѯ
    /// </summary>
    public class PageQuery<TEntity> : PageSortInfo, IPageQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// ָ����ѯ����
        /// </summary>
        protected Expression<Func<TEntity, bool>> Filter;

        /// <summary>
        /// ����һ���µ� <see cref="Query{TEntity}"/>
        /// </summary>
        public PageQuery()
        {

        }

        /// <summary>
        /// ����һ��ָ����ѯ������<see cref="Query{TEntity}"/>
        /// </summary>
        /// <param name="filter">ָ���Ĳ�ѯ����</param>
        public PageQuery(Expression<Func<TEntity, bool>> filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// ��ȡ��ѯ����
        /// </summary>
        public virtual Expression<Func<TEntity, bool>> GetFilter()
        {
            return Filter.And(this.GetQueryExpression());
        }
    }
}