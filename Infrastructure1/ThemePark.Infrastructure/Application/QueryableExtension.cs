using Abp.Domain.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Common;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// Class QueryableExtension.
    /// </summary>
    public static class QueryableExtension
    {
        #region Methods

        ///// <summary>
        ///// 异步返回 <see cref="T:System.Collections.Generic.List`1"/>
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> query)
        //{
        //    return await System.Data.Entity.QueryableExtensions.ToListAsync(query);
        //}

        /// <summary>
        /// To the query result.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>QueryResult&lt;TSource&gt;.</returns>
        public static QueryResult<TSource> ToQueryResult<TSource>(this IQueryable<TSource> query,
            QueryParameter<TSource> parameter)
            where TSource : class, new()
        {
            int total = 0;
            query = query.SetQueryParameter(parameter, out total);

            return new QueryResult<TSource>(query.ToList(), parameter.PageInfo, total);
        }

        /// <summary>
        /// To the query result.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="transFunc">The trans function.</param>
        /// <returns>QueryResult&lt;TResult&gt;.</returns>
        public static QueryResult<TResult> ToQueryResult<TSource, TResult>(this IQueryable<TSource> query,
            QueryParameter<TSource> parameter, Func<TSource, TResult> transFunc)
            where TSource : class, new()
            where TResult : class, new()
        {
            int total = 0;
            query = query.SetQueryParameter(parameter, out total);
            var result = query.ToList().Select(transFunc);

            return new QueryResult<TResult>(result.ToList(), parameter.PageInfo, total);
        }

        /// <summary>
        /// to query result as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task&lt;QueryResult&lt;TSource&gt;&gt;.</returns>
        public static async Task<QueryResult<TSource>> ToQueryResultAsync<TSource>(this IQueryable<TSource> query,
            QueryParameter<TSource> parameter)
            where TSource : class, new()
        {
            int total = 0;
            query = query.SetQueryParameter(parameter, out total);
            var list = await query.ToListAsync();

            return new QueryResult<TSource>(list, parameter.PageInfo, total);
        }

        /// <summary>
        /// to query result as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="transFunc">The trans function.</param>
        /// <returns>Task&lt;QueryResult&lt;TResult&gt;&gt;.</returns>
        public static async Task<QueryResult<TResult>> ToQueryResultAsync<TSource, TResult>(this IQueryable<TSource> query,
            QueryParameter<TSource> parameter, Func<TSource, TResult> transFunc)
            where TSource : class, new()
            where TResult : class, new()
        {
            int total = 0;
            query = query.SetQueryParameter(parameter, out total);
            var list = await query.ToListAsync();
            var result = list.Select(transFunc);

            return new QueryResult<TResult>(result.ToList(), parameter.PageInfo, total);
        }

        //public static QueryResult<TResult> ToQueryResult<TSource, TResult>(this IQueryable<TSource> query,
        //    QueryParameter<TSource, TResult> parameter)
        //    where TSource : class, new()
        //    where TResult : class, new()
        //{
        //    int total = 0;
        //    var result = query.SetQueryParameter(parameter, out total).ToList().Select(parameter.Transform);

        //    return new QueryResult<TResult>(result.ToList(), parameter.PageInfo, total);
        //}

        #endregion Methods

        #region Private

        /// <summary>
        /// Gets the order method.
        /// </summary>
        /// <param name="sortord">The sortord.</param>
        /// <param name="isFirst">if set to <c>true</c> [is first].</param>
        /// <returns>System.String.</returns>
        private static string GetOrderMethod(Sortord sortord, bool isFirst)
        {
            string str;

            switch (sortord)
            {
                case Sortord.Asc:
                    str = isFirst ? nameof(Queryable.OrderBy) : nameof(Queryable.ThenBy);
                    break;

                case Sortord.Desc:
                    str = isFirst ? nameof(Queryable.OrderByDescending) : nameof(Queryable.ThenByDescending);
                    break;

                default:
                    str = nameof(Queryable.OrderBy);
                    break;
            }

            return str;
        }

        /// <summary>
        /// Sets the order.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="isFirst">if set to <c>true</c> [is first].</param>
        /// <returns>IQueryable&lt;TSource&gt;.</returns>
        private static IQueryable<TSource> SetOrder<TSource>(IQueryable<TSource> query, SortField sort, bool isFirst)
        {
            Expression expression = Expression.Call(typeof(Queryable), GetOrderMethod(sort.Sortord, isFirst),
                new Type[] { query.ElementType, sort.Selector.Body.Type },
                new Expression[] { query.Expression, Expression.Quote(sort.Selector) });

            query = query.Provider.CreateQuery<TSource>(expression);

            return query;
        }

        /// <summary>
        /// Sets the query parameter.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="total">The total.</param>
        /// <returns>IQueryable&lt;TSource&gt;.</returns>
        private static IQueryable<TSource> SetQueryParameter<TSource>(this IQueryable<TSource> query,
            QueryParameter<TSource> parameter, out int total)
            where TSource : class, new()
        {
            //contact filters
            foreach (var filterParameter in parameter.FilterParameters)
            {
                query = query.Where(filterParameter.Predicate);
            }

            //sort
            bool isSorted = false;
            if (parameter.SortFields.Any())
            {
                for (int i = 0; i < parameter.SortFields.Count; i++)
                {
                    query = SetOrder(query, parameter.SortFields.ElementAt(i), i == 0);
                }

                isSorted = true;
            }
            else
            {
                //default order
                if (typeof(TSource).IsSubclassOfGenericInterface(typeof(IEntity<>)))
                {
                    var lambda = ExpressionHelper.CreatePropertySelectorLambda<TSource, IEntity<int>, int>(o => o.Id);
                    query = query.OrderBy((Expression<Func<TSource, int>>)lambda);
                    isSorted = true;
                }
            }

            total = query.Count();
            //pageable
            if (parameter.PageInfo != null && parameter.PageInfo.PageSize > 0 && isSorted)
            {
                query = query.Skip(parameter.PageInfo.PageIndex * parameter.PageInfo.PageSize).Take(parameter.PageInfo.PageSize);
            }

            return query;
        }

        #endregion Private
    }
}