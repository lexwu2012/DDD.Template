using Abp.AutoMapper;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 数据表同步记录新增输入
    /// </summary>
    [AutoMap(typeof(SyncTable))]
    public class SyncTableSaveNewInput
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 同步类型
        /// </summary>
        public SyncType SyncType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
