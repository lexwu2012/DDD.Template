using System;
using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 促销票类
    /// </summary>
    [AutoMap(typeof(Core.AgentTicket.AgencySaleTicketClass))]
    public class AgencySaleTicketClassOrderDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 公园id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>    
        [MapFrom(nameof(Core.AgentTicket.AgencySaleTicketClass.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public string SaleTicketClassName { get; set; }

        /// <summary>
        /// 票类所属公园名称
        /// </summary>
        [MapFrom(nameof(Core.AgentTicket.AgencySaleTicketClass.Park), nameof(Core.BasicData.Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 门市价
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 促销价格
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
        /// 开始销售时间
        /// </summary>    
        public DateTime? SaleStartDate { get; set; }

        /// <summary>
        /// 结束销售时间
        /// </summary>    
        public DateTime? SaleEndDate { get; set; }

        /// <summary>
        /// 购票数量
        /// </summary>
        public int? Qty { get; set; }
    }
}
