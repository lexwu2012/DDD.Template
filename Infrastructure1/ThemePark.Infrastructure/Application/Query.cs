using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Extensions;
using AutoMapper.QueryableExtensions;
using ThemePark.Common;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 查询扩展方法
    /// </summary>
    public static class Query
    {
        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        public static Task<TDto> FirstOrDefaultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery<TEntity> query)
            where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>();
            return data.FirstOrDefaultAsync();
        }


        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        /// <param name="sort">排序</param>
        /// <param name="dataAmount">取记录条数(默认10条)</param>
        /// <param name="defaultSort">默认排序</param>
        public static Task<List<TDto>> ToListAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery<TEntity> query, ISortInfo sort, int dataAmount = 10, string defaultSort = "Id Desc")
            where TEntity : class
        {
            return source.Where(query).ProjectTo<TDto>().OrderBy(sort, defaultSort).Take(dataAmount).ToListAsync();
        }

        /// <summary>
        /// 查询指定条件的数据
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
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        /// <param name="defaultSort">默认排序</param>
        public static Task<PageResult<TDto>> ToPageResultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IPageQuery<TEntity> query, string defaultSort = "Id Desc")
            where TEntity : class
        {
            return ToPageResultAsync<TEntity, TDto>(source, query, query, defaultSort);
        }

        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        /// <param name="page">分页信息</param>
        /// <param name="defaultSort">默认排序</param>
        public static async Task<PageResult<TDto>> ToPageResultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery<TEntity> query, IPageSortInfo page, string defaultSort = "Id Desc")
            where TEntity : class
        {
            page = page ?? new PageSortInfo();
            var pageIndex = Math.Max(0, page.PageIndex);
            var pageSize = Math.Max(1, page.PageSize);

            var result = new PageResult<TDto>() { PageIndex = pageIndex, PageSize = pageSize };
            var data = source.Where(query);
            result.TotalCount = await data.CountAsync();
            if (result.TotalCount > 0)
            {
                var mapData = data.ProjectTo<TDto>().OrderBy(page, defaultSort);
                //var temp = mapData.ToList();
                result.Data = await mapData.Skip(pageSize * pageIndex)
                    .Take(pageSize).ToListAsync();
            }

            return result;
        }

        /// <summary>
        /// 返回分页的数据数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source"></param>
        /// <param name="page">分页信息</param>
        /// <param name="defaultSort">默认排序</param>
        public static async Task<PageResult<TEntity>> ToPageResultAsync<TEntity>(this IQueryable<TEntity> source, IPageSortInfo page, string defaultSort = "Id Desc")
            where TEntity : class
        {
            page = page ?? new PageSortInfo();
            var pageIndex = Math.Max(0, page.PageIndex);
            var pageSize = Math.Max(1, page.PageSize);

            var result = new PageResult<TEntity>() { PageIndex = pageIndex, PageSize = pageSize };
            result.TotalCount = await source.CountAsync();
            if (result.TotalCount > 0)
            {
                var mapData = source.OrderBy(page, defaultSort);
                result.Data = await mapData.Skip(pageSize * pageIndex)
                    .Take(pageSize).ToListAsync();
            }

            return result;
        }

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
        /// 分页查询
        /// </summary>
        public static IQueryable<TEntity> PageBy<TEntity>(this IQueryable<TEntity> source, IPageSortInfo page, string defaultSort = "Id desc")
        {
            page = page ?? new PageSortInfo();
            var pageIndex = Math.Max(0, page.PageIndex);
            var pageSize = Math.Max(1, page.PageSize);

            return source.OrderBy(page, defaultSort).Skip(pageSize * pageIndex).Take(pageSize);
        }

        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <param name="source"></param>
        /// <param name="sort">排序</param>
        /// <param name="defaultSort">默认排序</param>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, ISortInfo sort, string defaultSort = "Id Desc")
        {
            var sortFields = sort?.SortFields?.Where(p => !string.IsNullOrEmpty(p)).ToArray();
            if (sortFields != null && sortFields.Length > 0)
            {
                source = source.OrderBy(string.Join(" , ", sortFields));
            }
            else if (!string.IsNullOrEmpty(defaultSort))
            {
                source = source.OrderBy(defaultSort);
            }
            else
            {
                source = source.OrderBy("Id DESC");
            }
            return source;
        }

        /// <summary>
        /// 获取查询表达式
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

        /// <summary>
        /// 获取应射的排序字段
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="mapType">映射的目标类型</param>
        public static IList<string> GetMapSortInfo(this ISortInfo sort, Type mapType)
        {
            if (sort?.SortFields == null || sort.SortFields.Count == 0) return new string[0];

            var sortList = new List<string>();
            var propertyMapType = typeof(MapFromAttribute);
            var properties = TypeDescriptor.GetProperties(mapType);

            foreach (var field in sort.SortFields)
            {
                // 考虑有使用 +/- 表示升序/降序的情况
                var split = field.Split(new[] { ' ' }, 2);
                var name = split.First().TrimStart('+', '-');
                var sorting = split.Length == 2
                    ? split[1]
                    : field.StartsWith("+") ? "Asc" : field.StartsWith("-") ? "Desc" : "Asc";

                var property = properties.Find(name, true);
                var attr = (MapFromAttribute)property?.Attributes[propertyMapType];
                if (attr?.PropertyPath != null && attr.PropertyPath.Length > 0)
                {
                    sortList.Add(string.Join(".", attr.PropertyPath) + " " + sorting);
                    continue;
                }

                sortList.Add(field);
            }

            return sortList;
        }

        /// <summary>
        /// 获取应射的排序字段
        /// </summary>
        /// <typeparam name="TMap">映射的目标类型</typeparam>
        /// <param name="sort"></param>
        public static IList<string> GetMapSortInfo<TMap>(this ISortInfo sort)
        {
            return GetMapSortInfo(sort, typeof(TMap));
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
                case QueryCompare.StartWidth:
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