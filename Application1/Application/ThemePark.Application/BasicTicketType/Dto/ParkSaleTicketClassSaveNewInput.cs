using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 促销票类新增输入
    /// </summary>
    [AutoMap(typeof(ParkSaleTicketClass))]
    public class ParkSaleTicketClassSaveNewInput
    {
        /// <summary>
        /// 促销名称
        /// </summary>    
        public string SaleTicketClassName { get; set; }

        /// <summary>
        /// 系统计算出门市价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 促销价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 基础票类Id
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 如果为null，取默认规则
        /// </summary>    
        public int? InParkRuleId { get; set; }

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
