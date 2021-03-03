using Abp.AutoMapper;
using AutoMapper.Configuration.Conventions;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 代理商促销票类新增input
    /// </summary>
    [AutoMapTo(typeof(AgencySaleTicketClass))]
    public class AgencySaleTicketClassSaveNewInput
    {
        /// <summary>
        /// 代理商编号
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 代理商促销票类模板Id
        /// </summary>
        public int AgencySaleTicketClassTemplateId { get; set; }

        /// <summary>
        /// 代理商促销票类模板名称
        /// </summary>    
        [MapTo(nameof(AgencySaleTicketClass.AgencySaleTicketClassName))]
        public string AgencySaleTicketClassName { get; set; }

        /// <summary>
        /// 团体类型Id
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 促销票类Id
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 电商门市价
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 代理商促销价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }

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
