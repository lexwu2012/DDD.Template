using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Web.CustomAttributes;
using System.Linq.Dynamic;


namespace DDD.Infrastructure.Web.Query
{
    public static class QueryHelper
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
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public static Task<TDto> FirstOrDefaultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery<TEntity> query) where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>();
            return data.FirstOrDefaultAsync();
        }

        /// <summary>
        /// 查询指定条件的数据并返回列表
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        public static Task<List<TDto>> ToListAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery<TEntity> query)
            where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>();
            return data.ToListAsync();
        }

        /// <summary>
        /// 根据条件组装查询表达式
        /// </summary>
        /// <typeparam name="TEntity">要查询的实体类型</typeparam>
        public static Expression<Func<TEntity, bool>> GetQueryExpression<TEntity>(this IQuery<TEntity> query)
            where TEntity : class
        {
            if (query == null) return null;

            var queryType = query.GetType();
            var param = Expression.Parameter(typeof(TEntity), "o");
            Expression body = null;
            foreach (PropertyInfo property in queryType.GetProperties())
            {
                var value = property.GetValue(query);
                if (value is string)
                {
                    var str = ((string)value).Trim();
                    value = string.IsNullOrEmpty(str) ? null : str;
                }

                Expression sub = null;

                foreach (var attribute in property.GetAttributes<QueryAttribute>())
                {
                    var propertyPath = attribute.PropertyPath;
                    if (propertyPath == null || propertyPath.Length == 0)
                        propertyPath = new[] { property.Name };

                    var experssion = CreateQueryExpression(param, value, propertyPath, attribute.Compare);
                    if (experssion != null)
                    {
                        sub = sub == null ? experssion : Expression.Or(sub, experssion);
                    }
                }

                if (sub != null)
                {
                    body = body == null ? sub : Expression.And(body, sub);
                }
            }

            if (body != null)
                return Expression.Lambda<Func<TEntity, bool>>(body, param);

            return null;
        }

        #region Private Method

        private static Expression CreateQueryExpression(Expression param, object value, string[] propertyPath,
            QueryCompare compare)
        {
            var member = CreatePropertyExpression(param, propertyPath);

            switch (compare)
            {
                case QueryCompare.Equal:
                    return CreateEqualExpression(member, value);
                case QueryCompare.NotEqual:
                    return CreateNotEqualExpression(member, value);
                case QueryCompare.Like:
                    return CreateLikeExpression(member, value);
                case QueryCompare.NotLike:
                    return CreateNotLikeExpression(member, value);
                case QueryCompare.StartWith:
                    return CreateStartsWithExpression(member, value);
                case QueryCompare.LessThan:
                    return CreateLessThanExpression(member, value);
                case QueryCompare.LessThanOrEqual:
                    return CreateLessThanOrEqualExpression(member, value);
                case QueryCompare.GreaterThan:
                    return CreateGreaterThanExpression(member, value);
                case QueryCompare.GreaterThanOrEqual:
                    return CreateGreaterThanOrEqualExpression(member, value);
                case QueryCompare.Between:
                    return CreateBetweenExpression(member, value);
                case QueryCompare.GreaterEqualAndLess:
                    return CreateGreaterEqualAndLessExpression(member, value);
                case QueryCompare.Include:
                    return CreateIncludeExpression(member, value);
                case QueryCompare.NotInclude:
                    return CreateNotIncludeExpression(member, value);
                case QueryCompare.IsNull:
                    return CreateIsNullExpression(member, value);
                case QueryCompare.HasFlag:
                    return CreateHasFlagExpression(member, value);
                default:
                    return null;
            }
        }

        private static MemberExpression CreatePropertyExpression(Expression param, string[] propertyPath)
        {
            var expression = propertyPath.Aggregate(param, Expression.Property) as MemberExpression;
            return expression;
        }


        private static Expression CreateEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = Expression.Constant(ChangeType(value, member.Type), member.Type);

            return Expression.Equal(member, val);
        }


        private static Expression CreateNotEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = Expression.Constant(ChangeType(value, member.Type), member.Type);

            return Expression.NotEqual(member, val);
        }

        private static Expression CreateLikeExpression(MemberExpression member, object value)
        {
            if (value == null) return null;
            if (member.Type != typeof(string))
                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");

            var str = value.ToString();
            var val = Expression.Constant(str);

            return Expression.Call(member, nameof(string.Contains), null, val);
        }

        private static Expression CreateNotLikeExpression(MemberExpression member, object value)
        {
            var like = CreateLikeExpression(member, value);
            if (like == null) return null;

            return Expression.Not(like);
        }

        private static Expression CreateStartsWithExpression(MemberExpression member, object value)
        {
            if (value == null) return null;
            if (member.Type != typeof(string))
                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");

            var str = value.ToString();
            var val = Expression.Constant(str);

            return Expression.Call(member, nameof(string.StartsWith), null, val);
        }

        private static Expression CreateLessThanExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = Expression.Constant(ChangeType(value, member.Type), member.Type);

            return Expression.LessThan(member, val);
        }

        private static Expression CreateLessThanOrEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = Expression.Constant(ChangeType(value, member.Type), member.Type);

            return Expression.LessThanOrEqual(member, val);
        }

        private static Expression CreateGreaterThanExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = Expression.Constant(ChangeType(value, member.Type), member.Type);

            return Expression.GreaterThan(member, val);
        }

        private static Expression CreateGreaterThanOrEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = Expression.Constant(ChangeType(value, member.Type), member.Type);

            return Expression.GreaterThanOrEqual(member, val);
        }

        private static Expression CreateBetweenExpression(MemberExpression member, object value)
        {
            var list = GetListValue(member.Type, value);
            if (list == null) return null;
            if (list.Count < 2) return null;

            var left = list[0];
            var right = list[list.Count - 1];
            if (left == null || right == null) return null;

            var leftVal = Expression.Constant(left, member.Type);
            var rightVal = Expression.Constant(right, member.Type);

            return Expression.And(Expression.GreaterThanOrEqual(member, leftVal),
                Expression.LessThanOrEqual(member, rightVal));
        }

        private static Expression CreateGreaterEqualAndLessExpression(MemberExpression member, object value)
        {
            var list = GetListValue(member.Type, value);
            if (list == null) return null;
            if (list.Count < 2) return null;

            var left = list[0];
            var right = list[list.Count - 1];
            if (left == null || right == null) return null;

            var leftVal = Expression.Constant(left, member.Type);
            var rightVal = Expression.Constant(right, member.Type);

            return Expression.And(Expression.GreaterThanOrEqual(member, leftVal),
                Expression.LessThan(member, rightVal));
        }

        private static Expression CreateIncludeExpression(MemberExpression member, object value)
        {
            var list = GetListValue(member.Type, value);
            if (list == null || list.Count == 0) return null;
            if (list.Count == 1)
            {
                return CreateEqualExpression(member, list[0]);
            }

            var vals = Expression.Constant(list);

            return Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { member.Type }, vals, member);
        }

        private static Expression CreateNotIncludeExpression(MemberExpression member, object value)
        {
            var includeExpression = CreateIncludeExpression(member, value);
            if (includeExpression == null) return null;

            return Expression.Not(includeExpression);
        }

        private static Expression CreateIsNullExpression(MemberExpression member, object value)
        {
            if (member.Type.IsValueType && !member.Type.IsNullableType())
                throw new InvalidOperationException($"Member:{member} can not use '{QueryCompare.IsNull}' compare");

            var nullVal = Expression.Constant(null, member.Type);

            if (value == null || Equals(value, false))
                return Expression.Equal(member, nullVal);

            return Expression.NotEqual(member, nullVal);
        }


        private static Expression CreateHasFlagExpression(MemberExpression member, object value)
        {
            if (!member.Type.GetNonNullableType().IsEnum)
                throw new InvalidOperationException($"Member:{member} is not a Enum type");
            var list = GetListValue(member.Type.GetNonNullableType(), value);
            if (list == null || list.Count == 0) return null;

            var p = member;
            if (member.Type.IsNullableType())
                p = Expression.Property(member, "Value");

            Expression exp = null;
            foreach (var item in list)
            {
                var val = Expression.Constant(item, typeof(Enum));
                var method = typeof(Enum).GetMethod(nameof(Enum.HasFlag), new[] { typeof(Enum) });
                Expression temp = Expression.Call(p, method, val);
                exp = exp != null ? Expression.Or(exp, temp) : temp;
            }

            return exp;
        }

        private static IList GetListValue(Type memberType, object value)
        {
            if (value == null) return null;
            var data = value as IEnumerable;
            if (value is string)
            {
                data = value.As<string>().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()).ToArray();
            }

            if (data == null)
            {
                data = new[] { value };
            }

            if (!data.Any()) return null;

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(memberType));

            foreach (var item in data)
            {
                try
                {
                    list.Add(ChangeType(item, memberType));
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }

            return list;
        }

        private static object ChangeType(object value, Type type)
        {
            if (value == null) return null;

            type = type.GetNonNullableType();
            if (type == value.GetType().GetNonNullableType()) return value;

            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, (string)value);
                else
                    return Enum.ToObject(type, value);
            }
            if (value is string && type == typeof(Guid))
                return new Guid((string)value);
            if (value is string && type == typeof(Version))
                return new Version((string)value);

            return Convert.ChangeType(value, type);
        }


        #endregion
    }
}
