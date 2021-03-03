using System;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 订单详情
    /// </summary>
    [AutoMapFrom(typeof(TOBody))]
    public class SearchVendorOrderDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get;set;}

        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(TOBody.Park), nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 主订单编号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MapFrom(nameof(TOBody.Customer), nameof(Customer.Pid))]
        public string Pid { get; set; }

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
        /// 实收金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// 有效起始时间
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader), nameof(TOHeader.ValidStartDate))]
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [MapFrom(nameof(TOBody.TOHeader), nameof(TOHeader.Agency), nameof(Agency.AgencyName))]
        public string AgencyName { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>
        [MapFrom(nameof(TOBody.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassName))]
        public string SaleTicketClassName { get; set; }
    }
}
