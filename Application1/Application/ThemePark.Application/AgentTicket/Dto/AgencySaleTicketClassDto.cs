using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 代理商促销票类传输实体
    /// </summary>
    [AutoMap(typeof(AgencySaleTicketClass))]
    public class AgencySaleTicketClassDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 团体类型编号
        /// </summary>    
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 基础票类编号
        /// </summary>    
        public int TicketClassId { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商促销票类模板Id
        /// </summary>
        public int AgencySaleTicketClassTemplateId { get; set; }
        /// <summary>
        /// 促销票类名称
        /// </summary>    

        public string SaleTicketClassName { get; set; }
        /// <summary>
        /// 电商门市价
        /// </summary>    

        public decimal Price { get; set; }
        /// <summary>
        /// 代理商促销价格
        /// </summary>    

        public decimal SalePrice { get; set; }
        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }
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
        /// <summary>
        /// 代理商
        /// </summary>
        public  AgencyDto Agency { get; set; }
        /// <summary>
        /// 代理商促销票类模板
        /// </summary>
        public  AgencySaleTicketClassTemplateDto AgencySaleTicketClassTemplate { get; set; }
    }
}
