using System;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.OTA.DTO
{
    [AutoMapFrom(typeof(TOVoucher))]
    public class OTASearchByPhoneDto
    {
        /// <summary>
        /// 主订单id
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.TOHeader), nameof(TOHeader.Id))]
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.Park), nameof(Park.ParkCode))]
        public short ParkCode { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.Park), nameof(Park.ParkName))]
        public string ParkName { get; set; }


        /// <summary>
        /// 代理商名称
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.TOHeader), nameof(TOHeader.Agency),nameof(Agency.AgencyName))]
        public string AgencyName { get; set; }

        /// <summary>
        /// 取票码
        /// </summary>
        public string TicketCode { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 订票时间
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.TOHeader), nameof(TOHeader.CreationTime))]
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// 入园时间
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.TOHeader), nameof(TOHeader.ValidStartDate))]
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 票类编号
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.AgencySaleTicketClassId))]
        public string AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 票类名称
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassName))]
        public string SaleTicketClassName { get; set; }


        /// <summary>
        /// 订票数量
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.Qty))]
        public int Qty { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [MapFrom(nameof(TOVoucher.TOBody), nameof(TOBody.OrderState))]
        public OrderState State { get; set; }
    }
}
