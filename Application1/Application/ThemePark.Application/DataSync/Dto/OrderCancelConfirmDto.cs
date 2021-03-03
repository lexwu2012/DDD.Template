namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 中心取消已经确认的订单
    /// </summary>
    public class OrderCancelConfirmDto
    {
        /// <summary>
        /// 同步到的公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 主订单Id
        /// </summary>
        public string TOHeaderId { get; set; }
    }
}
