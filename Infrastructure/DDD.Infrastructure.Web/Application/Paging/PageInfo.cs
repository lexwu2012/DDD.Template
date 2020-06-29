using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Web.Application.Paging
{
    public interface IPageInfo
    {
        int PageSize { get; set; }

        int PageIndex { get; set; }

        string Order { get; set; }

        string Field { get; set; }
    }

    /// <summary>
    /// 分页排序信息
    /// </summary>
    public class PageInfo : IPageInfo
    {
        /// <summary>
        /// 页号，从 0 开始
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 升序/降序（asc,desc，不传将默认为desc）
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// 排序字段，如（Id,Name.不传将默认为CreationTime）
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 初始化 <see cref="PageInfo" /> 类的新实例。
        /// </summary>
        public PageInfo(int pageIndex = 1, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
