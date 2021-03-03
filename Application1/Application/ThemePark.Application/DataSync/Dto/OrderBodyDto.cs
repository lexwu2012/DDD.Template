using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 子订单下发Dto
    /// </summary>
    public class OrderBodyDto
    {
        /// <summary>
        /// 代理商促销票类编号
        /// </summary>
        public int AgencySaleTicketClassId { get; set; }
        
        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustId { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 入园人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 票数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 商品单价（原价）
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 实际价格（售价）
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal? SettlementPrice { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        public decimal? ParkSettlementPrice { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 子订单状态
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}