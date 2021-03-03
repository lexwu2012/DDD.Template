using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(AgencySaleTicketClass))]
    public class AgencySaleTicketClassGetTicketClassDto
    {


        /// <summary>
        /// 门票类型
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClass.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassMode))]
        public TicketClassMode TicketClassMode { get; set; }


        /// <summary>
        /// 基础门票ID
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClass.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClassId))]
        public int TicketClassId { get; set; }


        /// <summary>
        /// 基础门票代码
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClass.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketTypeId))]
        public string TicketTypeId { get; set; }

        /// <summary>
        /// 公园促销票类编号
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }


    }
}
