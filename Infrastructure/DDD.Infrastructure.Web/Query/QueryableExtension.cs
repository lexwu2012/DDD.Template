using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DDD.Infrastructure.Query;
using System.Data.Entity;


namespace DDD.Infrastructure.Web.Query
{
    public static class QueryableExtension
    {
        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="query"></param>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, IQuery<TEntity> query)
            where TEntity : class
        {
            var filter = query?.GetFilter();
            if (filter != null)
                source = source.Where(filter);

            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="source"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Task<TDto> FirstOrDefaultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery<TEntity> query) where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>();
            return data.FirstOrDefaultAsync();
        }
    }
}
