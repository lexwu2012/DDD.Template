using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ThemePark.Common
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// 返回分页后的数据
        /// </summary>
        /// <param name="query">查询的数据集</param>
        /// <param name="pageIndex">第几页，从1开始</param>
        /// <param name="pageSize">每页多少条数据，不能小于1</param>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", "页不能小于1");

            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageSize", "每页大小不能小于1");

            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 如果条件 <paramref name="condition"/> 为 true， 根据指定的过滤器 <paramref name="predicate"/> 筛选集合
        /// </summary>
        /// <param name="query">查询的数据集</param>
        /// <param name="condition">是否过滤</param>
        /// <param name="predicate">过滤器</param>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 如果条件 <paramref name="condition"/> 为 true， 根据指定的过滤器 <paramref name="predicate"/> 筛选集合
        /// </summary>
        /// <param name="query">查询的数据集</param>
        /// <param name="condition">是否过滤</param>
        /// <param name="predicate">过滤器</param>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }
        
        /// <summary>
        /// 拼接 and 条件语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null) return second;
            if (second == null) return first;

            return ParameterRebinder.Compose(first, second, Expression.And);
        }

        /// <summary>
        /// 拼接 or 条件语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null) return second;
            if (second == null) return first;

            return ParameterRebinder.Compose(first, second, Expression.Or);
        }
    }

    #region ParameterRebinder

    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        public static Expression<T> Compose<T>(Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)  
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first  
            var secondBody = ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression   
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
    #endregion
}
