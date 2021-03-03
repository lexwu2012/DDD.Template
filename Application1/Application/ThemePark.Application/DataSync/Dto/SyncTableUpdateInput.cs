using Abp.AutoMapper;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 数据表同步记录数据更新输入
    /// </summary>
    [AutoMap(typeof(SyncTable))]
    public class SyncTableUpdateInput : SyncTableSaveNewInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
    }
}
