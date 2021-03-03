using System;
using Abp.AutoMapper;
using ThemePark.Common;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Refund.Dto
{
    /// <summary>
    /// 票详情
    /// </summary>
    public class TicketRefundDetailDto
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>    
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 票状态
        /// </summary>
        public TicketSaleStatus TicketSaleStatus { get; set; }

        /// <summary>
        /// 票状态
        /// </summary>
        public string TicketSaleStatusName => TicketSaleStatus.DisplayName();

        /// <summary>
        /// 促销票类名称
        /// </summary>    
        public virtual string SaleTicketClassName { get; set; }

        /// <summary>
        /// 门市价
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 促销价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 销售时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 票类型（散客，团体）
        /// </summary>
        public string TicketCatogory { get; set; }

        /// <summary>
        /// 门票类型
        /// </summary>
        public virtual TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public int FlowId { get; set; }
    }

    [AutoMapFrom(typeof(NonGroupTicket))]
    public class NonGroupTicketRefundDetailDto : TicketRefundDetailDto
    {
        [MapFrom(nameof(NonGroupTicket.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public string SaleTicketClassName { get; set; }
        [MapFrom(nameof(NonGroupTicket.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassMode))]
        public TicketClassMode TicketClassMode { get; set; }
    }

    [AutoMapFrom(typeof(GroupTicket))]
    public class GroupTicketRefundDetailDto : TicketRefundDetailDto
    {
        [MapFrom(nameof(GroupTicket.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public override string SaleTicketClassName { get; set; }
        [MapFrom(nameof(GroupTicket.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassMode))]
        public override TicketClassMode TicketClassMode { get; set; }
    }

    [AutoMapFrom(typeof(OtherNonGroupTicket))]
    public class OtherNonGrouptTicketRefundDetailDto : TicketRefundDetailDto
    {
        [MapFrom(nameof(OtherNonGroupTicket.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public override string SaleTicketClassName { get; set; }
        [MapFrom(nameof(OtherNonGroupTicket.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassMode))]
        public override TicketClassMode TicketClassMode { get; set; }
    }
}