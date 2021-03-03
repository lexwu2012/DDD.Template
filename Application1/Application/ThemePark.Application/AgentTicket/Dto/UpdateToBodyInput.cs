using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 修改旅行社预订订单体
    /// </summary>
    [AutoMap(typeof(TOBodyDto))]
    public class UpdateToBodyInput
    {
        /// <summary>
        /// 公园编号
        /// </summary>    
        [Required(ErrorMessage = "预订公园为必填项")]
        [DisplayName("公园：")]
        public int ParkId { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>    
        [Required(ErrorMessage = "可订票类为必填项")]
        [DisplayName("可订票类：")]
        public int AgencyParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 票数量
        /// </summary>    
        [Required]
        [Range(1, int.MaxValue)]
        public int? Qty { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>    
        [Required]
        public System.DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>    
        public int? CustId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }
    }
}
