namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 订单核销Dto
    /// </summary>
    public class OrderConsumeDto
    {
        /// <summary>
        /// 子订单号（OTA数据同步用到）
        /// </summary>
        public string SubOrderid { get; set; }

        /// <summary>
        /// 主订单Id（旅行社数据同步用到）
        /// </summary>        
        public string TOHeaderId { get; set; }
    }
}