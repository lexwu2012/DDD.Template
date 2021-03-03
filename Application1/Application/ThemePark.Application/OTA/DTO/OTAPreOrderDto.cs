namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 预订返回
    /// </summary>
    public class OTAPreOrderDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 票类ID
        /// </summary>
        public int AgencySaleTicketClassId { get; set; }
    }
}
