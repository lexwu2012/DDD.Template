using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 获取网络订单
    /// </summary>
    [AutoMapFrom(typeof(TOBody))]
    public class GetWebTicketOrderDto
    {
        /// <summary>
        /// 子订单订单编号
        /// </summary>
        [MapFrom(nameof(TOBody.Id))]
        public string OrderId { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader), nameof(TOHeader.Agency), nameof(Agency.AgencyName))]
        public string AgencyName { get; set; }


        /// <summary>
        /// 促销票类
        /// </summary>
        [MapFrom(nameof(TOBody.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassName))]
        public string AgencySaleTicketClassName { get; set; }

        /// <summary>
        /// 促销票类
        /// </summary>
        [MapFrom(nameof(TOBody.Customer), nameof(Customer.CustomerName))]
        public string CustomerName { get; set; }

        /// <summary>
        /// 促销票类
        /// </summary>
        [MapFrom(nameof(TOBody.Customer), nameof(Customer.Pid))]
        public string Pid { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader), nameof(TOHeader.ValidStartDate))]
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 购票数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        [MapFrom(nameof(TOBody.ParkId))]
        public int ParkId { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(TOBody.Park),nameof(Park.ParkName))]
        public string ParkName { get; set; }
    }
}
