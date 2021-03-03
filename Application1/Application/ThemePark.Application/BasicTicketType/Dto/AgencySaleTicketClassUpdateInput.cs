using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 代理商促销票类更改input
    /// </summary>
    [AutoMap(typeof(AgencySaleTicketClass))]
    public class AgencySaleTicketClassUpdateInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商促销票类名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string AgencySaleTicketClassName { get; set; }

        /// <summary>
        /// 电商门市价
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        [RegularExpression(@"^(0|[1-9][0-9]{0,9})(\.[0-9]{1,2})?$", ErrorMessage = "输入必须为大于0的数字.")]
        public decimal Price { get; set; }

        /// <summary>
        /// 代理商促销价格
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
