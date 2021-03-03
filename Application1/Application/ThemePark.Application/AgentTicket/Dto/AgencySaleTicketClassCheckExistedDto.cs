using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(AgencySaleTicketClass))]
    public class AgencySaleTicketClassCheckExistedDto
    {
        /// <summary>
        /// 代理商编号
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClass.Agency),nameof(Agency.AgencyName))]
        public string AgencyName { get; set ; }

        /// <summary>
        /// 代理商促销票类名称
        /// </summary>
        public string AgencySaleTicketClassName { get; set; }

        /// <summary>
        /// 代理商促销票类模板Id
        /// </summary>
        public int AgencySaleTicketClassTemplateId { get; set; }

        /// <summary>
        /// 代理商促销模板名称
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClass.AgencySaleTicketClassTemplate), nameof(AgencySaleTicketClassTemplate.AgencySaleTicketClassTemplateName))]
        public string AgencySaleTicketClassTemplateName { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClass.Park), nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 结束销售时间
        /// </summary>
        public System.DateTime? SaleEndDate { get; set; }

        /// <summary>
        /// 开始销售时间
        /// </summary>
        public System.DateTime? SaleStartDate { get; set; }
    }
}
