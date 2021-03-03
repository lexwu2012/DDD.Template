namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡卡内信息
    /// </summary>
    public class VIPCardBasicInput
    {
        /// <summary>
        /// ID
        /// </summary>
        public int IcBasicInfoId { get; set; }
        /// <summary>
        /// IC卡内码
        /// </summary>
        public string Icno { get; set; }
        /// <summary>
        /// 票类Id
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 基础票类ID
        /// </summary>
        public int TicketClassId { get; set; }
    }
}
