﻿using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 散客销售数据
    /// </summary>
    [AutoMap(typeof(TOTicket))]
    public class TOTicketSaleInfo
    {
        /// <summary>
        /// 卖方FromParkId
        /// </summary>    
        public int ParkId { get; set; }

        /// <summary>
        /// 门票数量
        /// </summary>    
        public int Qty { get; set; }

        /// <summary>
        /// 基础票类名称
        /// </summary>
        [MapFrom(nameof(TOTicket.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassTemplate), nameof(AgencySaleTicketClassTemplate.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassName))]
        public string TicketClassName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 入园总次数
        /// </summary>
        public int InparkCounts { get; set; }
    }
}
