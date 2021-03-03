using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 门市价前端更改页面能更改的字段
    /// </summary>
    [AutoMapTo(typeof(ParkSaleTicketClass))]
    public class ParkSaleTicketClassInput
    {
        /// <summary>
        /// SaleTicketClassName
        /// </summary>    
        public string SaleTicketClassName { get; set; }

        /// <summary>
        /// 门市价（2017/12/18需求改为门市价可更改）
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// 促销价格
        /// </summary>    
        [Required]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// IsEveryday
        /// </summary>    
        public bool? IsEveryday { get; set; }

        /// <summary>
        /// SaleStartDate
        /// </summary>    
        public System.DateTime? SaleStartDate { get; set; }

        /// <summary>
        /// SaleEndDate
        /// </summary>    
        public System.DateTime? SaleEndDate { get; set; }

        /// <summary>
        /// IsSupportSingle
        /// </summary>    
        public bool? IsSupportSingle { get; set; }

        /// <summary>
        /// IsSupportGroup
        /// </summary>    
        public bool? IsSupportGroup { get; set; }

        /// <summary>
        /// IsSupportOnline
        /// </summary>    
        public bool? IsSupportOnline { get; set; }

        /// <summary>
        /// 如果为null，取默认规则
        /// </summary>    
        public int? InParkRuleId { get; set; }

        /// <summary>
        /// 售票时是否绑定用户
        /// </summary>    
        public bool? IsBindingCust { get; set; }

        /// <summary>
        /// 在售、下架
        /// </summary>    
        public TicketClassStatus TicketClassStatus { get; set; }

        /// <summary>
        /// Remark
        /// </summary>    
        public string Remark { get; set; }
    }
}