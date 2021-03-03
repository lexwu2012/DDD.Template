using System.Collections.Generic;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// ��ҳ������Ϣ
    /// </summary>
    public interface IPageSortInfo : IPageInfo, ISortInfo
    {
        
    }

    /// <summary>
    /// ��ҳ������Ϣ
    /// </summary>
    public class PageSortInfo : PageInfo, IPageSortInfo
    {
        /// <summary>
        /// ��ҳ������Ϣ
        /// </summary>
        public PageSortInfo()
        {
            SortFields = new List<string>();
        }

        /// <summary>
        /// ��ҳ������Ϣ
        /// </summary>
        public PageSortInfo(int pageIndex, int pageSize = 10, IEnumerable<string> sort = null)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;

            var sortFields = new List<string>();
            if (sort != null) sortFields.AddRange(sort);

            SortFields = sortFields;
        }

        /// <summary>
        /// �����ֶ�
        /// </summary>
        /// <example>['SortNo desc', 'CreateTime desc']</example>
        public IList<string> SortFields { get; set; }
    }
}