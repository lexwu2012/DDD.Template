using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ThemePark.Infrastructure.Application
{
    public static class DropdownDtoExtension
    {
        #region Methods

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>DropdownDto.</returns>
        public static DropdownDto ToDropdownDto(this IEnumerable<DropdownItem> collection)
        {
            return new DropdownDto(collection);
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>DropdownDto.</returns>
        public static DropdownDto ToDropdownDto(this IEnumerable<DropdownItem<int>> collection)
        {
            return new DropdownDto(collection);
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>DropdownDto&lt;TPrimaryKey&gt;.</returns>
        public static DropdownDto<TPrimaryKey> ToDropdownDto<TPrimaryKey>(this IEnumerable<DropdownItem<TPrimaryKey>> collection) where TPrimaryKey : struct
        {
            return new DropdownDto<TPrimaryKey>(collection);
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns>DropdownDto.</returns>
        public static DropdownDto ToDropdownDto(this IQueryable<DropdownItem> queryable)
        {
            return queryable.ToList().ToDropdownDto();
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>DropdownDto.</returns>
        public static DropdownDto ToDropdownDto<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, DropdownItem>> selector)
        {
            return queryable.Select(selector).Distinct().ToDropdownDto();
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns>DropdownDto.</returns>
        public static DropdownDto<TPrimaryKey> ToDropdownDto<TPrimaryKey>(this IQueryable<DropdownItem<TPrimaryKey>> queryable) where TPrimaryKey : struct
        {
            return queryable.ToList().ToDropdownDto();
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>DropdownDto.</returns>
        public static DropdownDto<TPrimaryKey> ToDropdownDto<TEntity, TPrimaryKey>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, DropdownItem<TPrimaryKey>>> selector) where TPrimaryKey : struct
        {
            return queryable.Select(selector).Distinct().ToDropdownDto();
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns>DropdownDto.</returns>
        public static async Task<DropdownDto> ToDropdownDtoAsync(this IQueryable<DropdownItem> queryable)
        {
            var list = await queryable.ToListAsync();
            return list.ToDropdownDto();
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>DropdownDto.</returns>
        public static Task<DropdownDto> ToDropdownDtoAsync<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, DropdownItem>> selector)
        {
            return queryable.Select(selector).Distinct().ToDropdownDtoAsync();
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns>DropdownDto.</returns>
        public static async Task<DropdownDto<TPrimaryKey>> ToDropdownDtoAsync<TPrimaryKey>(this IQueryable<DropdownItem<TPrimaryKey>> queryable) where TPrimaryKey : struct
        {
            var list = await queryable.ToListAsync();
            return list.ToDropdownDto();
        }

        /// <summary>
        /// To the dropdown dto.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>DropdownDto.</returns>
        public static Task<DropdownDto<TPrimaryKey>> ToDropdownDtoAsync<TEntity, TPrimaryKey>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, DropdownItem<TPrimaryKey>>> selector) where TPrimaryKey : struct
        {
            return queryable.Select(selector).Distinct().ToDropdownDtoAsync();
        }

        #endregion Methods
    }
}