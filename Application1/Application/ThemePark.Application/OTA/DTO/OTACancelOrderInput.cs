namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 取消订单参数
    /// </summary>
    public class OTACancelOrderInput:ValidateParams
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

    }
}
