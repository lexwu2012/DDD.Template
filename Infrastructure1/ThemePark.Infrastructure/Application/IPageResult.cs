using System;
using System.Collections.Generic;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 分页结果
    /// </summary>
    public interface IPageResult : IPageInfo
    {
        /// <summary>
        /// 总数
        /// </summary>
        int TotalCount { get; set; }
    }

    /// <summary>
    /// 分页返回结果
    /// </summary>
    public class PageResult<TData> : ListResult<TData>, IPageResult
    {
        /// <summary>
        /// 分页返回结果
        /// </summary>
        public PageResult()
        {

        }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        public PageResult(IList<TData> data, int totalCount, int pageIndex, int pageSize)
            : base(data)
        {
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
            Data = data;
        }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        public PageResult(IList<TData> data, int totalCount, IPageInfo pager)
            : this(data, totalCount, pager?.PageIndex ?? 0, pager?.PageSize ?? 0)
        {
        }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        public PageResult(ResultCode code, string message = null)
            : base(code, message)
        {

        }

        /// <summary>
        /// 当前页
        /// </summary>
        /// <example>1</example>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        /// <example>30</example>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        /// <example>5</example>
        public int TotalPage => PageSize > 0 ? (int)Math.Ceiling((decimal)TotalCount / PageSize) : 0;

        /// <summary>
        /// 数据总条数
        /// </summary>
        /// <example>250</example>
        public int TotalCount { get; set; }
    }
}
