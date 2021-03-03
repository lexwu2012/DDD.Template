using System.Collections.Generic;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public interface ISortInfo
    {
        /// <summary>
        /// �����ֶ�
        /// </summary>
        /// <example>['SortNo desc', 'CreateTime desc']</example>
        IList<string> SortFields { get; set; }
    }
}