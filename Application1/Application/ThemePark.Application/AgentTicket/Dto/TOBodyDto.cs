using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 旅行社订单体
    /// </summary>
    [AutoMap(typeof(TOBody))]
    public class TOBodyDto : FullAuditedEntityDto<string>
    {
        /// <summary>
        /// 主订单编号
        /// </summary> 
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 子订单状态
        /// </summary>
        public OrderState OrderState { get; set; }

        /// <summary>
        /// 序号
        /// </summary>  
        public int Seq { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 入园人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 票数量
        /// </summary>  
        public int Qty { get; set; }

        /// <summary>
        /// 商品单价
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 实际价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        public decimal ParkSettlementPrice { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>    
        public decimal Amount { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
