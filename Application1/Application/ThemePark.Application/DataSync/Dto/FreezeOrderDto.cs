using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 订单冻结dto
    /// </summary>
    public class FreezeOrderDto : OrderCancelConfirmDto
    {
        /// <summary>
        /// 同步类型
        /// </summary>
        public DataSyncType DataSyncType { get; set; }
    }
}
