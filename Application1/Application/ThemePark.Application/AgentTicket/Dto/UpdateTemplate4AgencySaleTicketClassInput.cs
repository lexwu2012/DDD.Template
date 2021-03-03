using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 更新模板所更改的代理商促销票类内容
    /// </summary>
    [AutoMap(typeof(AgencySaleTicketClass))]
    public class UpdateTemplate4AgencySaleTicketClassInput
    {
        /// <summary>
        /// 更改后的代理商促销票类名称
        /// </summary>
        public string AgencySaleTicketClassName { get; set; }

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
        /// 销售开始日期
        /// </summary>
        public DateTime SaleStartDate { get; set; }

        /// <summary>
        /// 销售结束时间
        /// </summary>
        public DateTime SaleEndDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TicketClassStatus Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
