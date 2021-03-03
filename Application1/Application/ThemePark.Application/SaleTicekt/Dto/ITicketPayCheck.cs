using System.Collections.Generic;
using ThemePark.Application.Trade.Dto;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 团体票支付数据验证
    /// </summary>
    public interface ISaleTicketCheck<TTicket> 
        where TTicket:ITicketInfo
    {
        /// <summary>
        /// 票类详情
        /// </summary>
        List<TTicket> TicketInfos { get; set; }

        /// <summary>
        /// 支付详情
        /// </summary>
        TradeInfoInput TradeInfos { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        InvoiceInput InvoiceInfos { get; set; }

        /// <summary>
        /// 旅行社订单号
        /// </summary>
        string OrderId { get; set; }
    }

   
}
