using Abp.AutoMapper;
using System;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 团体购票详情
    /// </summary>
    [AutoMapTo(typeof(GroupTicket))]
    class GroupTicketDetailDto
    {
        /// <summary>
        /// 代理商编号
        /// </summary>    
        [MapFrom(nameof(GroupTicket.Agency),nameof(Agency.AgencyName))]
        public string AgencyName { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        [MapFrom(nameof(GroupTicket.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassTemplate),nameof(AgencySaleTicketClassTemplate.AgencyTypeId))]
        public int AgencyTypeId { get; set; }

        /// <summary>
        /// 团体类型名称
        /// </summary>
        [MapFrom(nameof(GroupTicket.GroupType), nameof(GroupType.TypeName))]
        public string GroupTypeName { get; set; }

        /// <summary>
        /// 条码号
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// 基础票类ID
        /// </summary>
        [MapFrom(nameof(GroupTicket.AgencySaleTicketClass), nameof(AgencySaleTicketClass.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.Id))]
        public int TicketClassId { get; set; }

        /// <summary>
        /// 旅行社预订订单号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>    
        public string Tradeno { get; set; }

        /// <summary>
        /// 门票状态：0 预售 1 有效 2 已入园 3 作废 有效 可能是部分入园
        /// </summary>    
        public TicketSaleStatus State { get; set; }

        /// <summary>
        /// 基础票类名称
        /// </summary>    
        [MapFrom(nameof(GroupTicket.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassTemplate), nameof(AgencySaleTicketClassTemplate.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]        
        public string SaleTicketClassName { get; set; }

        /// <summary>
        /// 票人数
        /// </summary>    
        [MapFrom(nameof(GroupTicket.AgencySaleTicketClass), nameof(AgencySaleTicketClass.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketType), nameof(TicketType.Persons))]
        public int Persons { get; set; }


        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int ParkSaleTicketClassId { get; set; }

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
        /// 有效天数
        /// </summary>    
        public int ValidDays { get; set; }

        /// <summary>
        /// 计划开始使用日期
        /// </summary>    
        public System.DateTime ValidStartDate { get; set; }


        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(NonGroupTicket.Park), nameof(Park.ParkName))]
        public string ParkName { get; set; }
    }
}
