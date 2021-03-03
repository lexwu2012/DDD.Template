using Abp.AutoMapper;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 公园同步记录更改保存输入
    /// </summary>
    [AutoMap(typeof(SyncPark))]
    public class SyncParkUpdateInput : SyncParkSaveNewInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        public string ParkName { get; set; }
    }
}
