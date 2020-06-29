using DDD.Infrastructure.Web.Application.Paging;
using DDD.Infrastructure.Web.Query;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.MongoDb.Extensions
{
    public static class IMongoCollectionExtension
    {
        // <summary>
        /// 同步查询指定条件的数据，并且返回指定类型TDto
        /// </summary>
        /// <typeparam name="TEntity">查询实体</typeparam>
        /// <typeparam name="TDto">返回类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query"></param>
        public static IFindFluent<TEntity, TDto> FindSync<TEntity, TDto>(this IMongoCollection<TEntity> source, IQuery<TEntity> query)
            where TEntity : class
        {
            var projection = GetTDtoReturnProperties<TEntity, TDto>();

            var expression = query?.GenerateExpression();
            if (null == expression)
            {
                var emptyExpression = Builders<TEntity>.Filter.Empty;
                return source.Find(emptyExpression).Project<TDto>(projection);
            }

            return source.Find(expression).Project<TDto>(projection);
        }

        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        /// <param name="page">分页信息</param>
        public static async Task<PageResult<TDto>> ToPageResultAsync<TEntity, TDto>(this IMongoCollection<TEntity> source, IQuery<TEntity> query, IPageInfo page)
            where TEntity : class
        {
            var pageIndex = Math.Max(0, page.PageIndex);
            var pageSize = Math.Max(1, page.PageSize);

            var cursor = source.FindSync<TEntity, TDto>(query);

            var pageResult = new PageResult<TDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = (int)await cursor.CountDocumentsAsync(),
                Data = await cursor.Skip(pageSize * (pageIndex - 1)).Limit(pageSize).Sort(page).ToListAsync()
            };

            return pageResult;
        }

        /// <summary>
        /// 排序方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortInfo"></param>
        /// <returns></returns>
        public static IFindFluent<TEntity, TDto> Sort<TEntity, TDto>(this IFindFluent<TEntity, TDto> source, IPageInfo sortInfo)
            where TEntity : class
        {
            var sort = Builders<TEntity>.Sort;
            SortDefinition<TEntity> sortDefinition = null;
            if (sortInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(sortInfo.Order) && !string.IsNullOrWhiteSpace(sortInfo.Field))
                {
                    if (sortInfo.Order.Contains("asc"))
                        sortDefinition = sort.Ascending(sortInfo.Field);
                    if (sortInfo.Order.Contains("desc"))
                        sortDefinition = sort.Descending(sortInfo.Field);
                }
            }

            return source.Sort(sortDefinition);
        }

        /// <summary>
        /// 获取指定的返回列
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        private static ProjectionDefinition<TEntity, TDto> GetTDtoReturnProperties<TEntity, TDto>()
            where TEntity : class
        {
            var returnType = typeof(TDto);

            var fieldList = new List<ProjectionDefinition<TEntity>>();
            foreach (var property in returnType.GetProperties())
            {
                fieldList.Add(Builders<TEntity>.Projection.Include(property.Name));
            }

            return Builders<TEntity>.Projection.Combine(fieldList);
        }
    }
}
