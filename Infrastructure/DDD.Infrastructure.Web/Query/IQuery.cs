using System;
using System.Linq.Expressions;

namespace DDD.Infrastructure.Web.Query
{
    public interface IQuery<TEntity> where TEntity : class
    {

        /// <summary>
        /// 获取查询条件
        /// </summary>
        Expression<Func<TEntity, bool>> GetFilter();
    }
}
