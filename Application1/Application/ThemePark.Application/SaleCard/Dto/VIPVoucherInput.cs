namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡凭证输入参数
    /// </summary>
    public class VIPVoucherInput
    {
        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 实际销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 有效天数
        /// </summary>
        public int VoucherValidays { get; set; }

    }
}
