using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 代理商促销票类dto
    /// </summary>
    [AutoMap(typeof(AgencySaleTicketClassTemplate))]
    public class AgencySaleTicketClassTemplateDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 公园编号
        /// </summary>    
        public int ParkId { get; set; }


        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClassTemplate.Park),nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 代理商类型编号
        /// </summary>
        public int AgencyTypeId { get; set; }

        /// <summary>
        /// 代理商类型名称
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClassTemplate.AgencyType),nameof(AgencyType.AgencyTypeName))]
        public string AgencyTypeName { get; set; }

        /// <summary>
        /// 团体类型Id
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 团体类型名称
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClassTemplate.GroupType), nameof(GroupType.TypeName))]
        public string GroupTypeName { get; set; }

        /// <summary>
        /// 公园促销票类编号
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 团体类型名称
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClassTemplate.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public string SaleTicketClassName { get; set; }


        /// <summary>
        /// 门票类型
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClassTemplate.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassMode))]
        public TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// 代理商促销票类名称
        /// </summary>    
        public string AgencySaleTicketClassTemplateName { get; set; }

        /// <summary>
        /// 电商门市价
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 电商促销价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal? SettlementPrice { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        public decimal? ParkSettlementPrice { get; set; }

        /// <summary>
        /// 开始销售时间
        /// </summary>    
        public System.DateTime? SaleStartDate { get; set; }

        /// <summary>
        /// 结束销售时间
        /// </summary>    
        public System.DateTime? SaleEndDate { get; set; }

        /// <summary>
        /// 状态：在售、下架
        /// </summary>    
        public TicketClassStatus Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }
    }
}
