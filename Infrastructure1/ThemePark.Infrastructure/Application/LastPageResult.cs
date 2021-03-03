using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 增量分页数据
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class LastPageResult<TData> : ListResult<TData>, IHasTotalCount, IPageByLast
    {
        /// <summary>
        /// 增量分页数据
        /// </summary>
        public LastPageResult()
        {

        }

        /// <summary>
        /// 增量分页数据
        /// </summary>
        public LastPageResult(IList<TData> data, int totalCount, int? lastId, int pageSize)
            : base(data)
        {
            TotalCount = totalCount;
            LastId = lastId;
            PageSize = pageSize;
            Data = data;
        }

        /// <summary>
        /// 下页数据Id
        /// </summary>
        /// <remarks>
        /// 如果为 null 表示后面没有数据了
        /// </remarks>
        public int? LastId { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 数据总条数
        /// </summary>
        /// <example>250</example>
        public int TotalCount { get; set; }
    }
}