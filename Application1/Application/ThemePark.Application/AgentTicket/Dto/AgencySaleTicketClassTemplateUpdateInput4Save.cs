using Abp.AutoMapper;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 代理商促销票类模板更改输入
    /// </summary>
    [AutoMap(typeof(AgencySaleTicketClassTemplate))]
    public class AgencySaleTicketClassTemplateUpdateInput4Save
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>    
        [DisplayName("促销票类名称：")]
        [Required(ErrorMessage = "必填字段")]
        [StringLength(50)]
        public string AgencySaleTicketClassTemplateName { get; set; }

        /// <summary>
        /// 代理商促销票类Id
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 开始销售时间
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        public DateTime SaleStartDate { get; set; }

        /// <summary>
        /// 结束销售时间
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        public DateTime SaleEndDate { get; set; }

        /// <summary>
        /// 门市价
        /// </summary>
        [Required(ErrorMessage = "必填字段")]
        [RegularExpression(@"^(0|[1-9][0-9]{0,9})(\.[0-9]{1,2})?$", ErrorMessage = "输入必须为大于0的数字.")]
        public decimal Price { get; set; }

        /// <summary>
        /// 促销价
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        [RegularExpression(@"^(0|[1-9][0-9]{0,9})(\.[0-9]{1,2})?$", ErrorMessage = "输入必须为大于0的数字.")]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        [Required(ErrorMessage = "必填字段")]
        [RegularExpression(@"^(0|[1-9][0-9]{0,9})(\.[0-9]{1,2})?$", ErrorMessage = "输入必须为大于0的数字.")]
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        [Required(ErrorMessage = "必填字段")]
        [RegularExpression(@"^(0|[1-9][0-9]{0,9})(\.[0-9]{1,2})?$", ErrorMessage = "输入必须为大于0的数字.")]
        public decimal ParkSettlementPrice { get; set; }

        /// <summary>
        /// 在售、下架
        /// </summary>    
        public TicketClassStatus Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }
    }
}
