using System;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 电商订单详情
    /// </summary>
    [AutoMapFrom(typeof(TOBody))]
    public class OTAOrderDetailDto
    {
        
        /// <summary>
        /// 主订单id
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader),nameof(TOHeader.Id))]
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        public short ParkCode { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 取票码
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader), nameof(TOHeader.Id))]
        public string TicketCode { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [MapFrom(nameof(TOBody.Customer), nameof(Customer.Pid))]
        public string Pid { get; set; }

        /// <summary>
        /// 订票时间
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader), nameof(TOHeader.CreationTime))]
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// 入园时间
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader), nameof(TOHeader.ValidStartDate))]
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 票类编号
        /// </summary>
        [MapFrom(nameof(TOBody.AgencySaleTicketClassId))]
        public string AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 基础票种ID
        /// </summary>
        [MapFrom(nameof(TOBody.AgencySaleTicketClass),nameof(AgencySaleTicketClass.ParkSaleTicketClass),nameof(ParkSaleTicketClass.TicketClass),nameof(TicketClass.TicketTypeId))]
        public string TicketTypeId { get; set; }

        /// <summary>
        /// 订票数量
        /// </summary>
        [MapFrom(nameof(TOBody.Qty))]
        public int Qty { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [MapFrom(nameof(TOBody.OrderState))]
        public OrderState State { get; set; }

    }


}
