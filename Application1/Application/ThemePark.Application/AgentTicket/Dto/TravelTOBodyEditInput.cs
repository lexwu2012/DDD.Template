using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 子订单更改
    /// </summary>
    [AutoMapTo(typeof(TOBodyPre))]
    public class TravelTOBodyEditInput
    {
        /// <summary>
        /// 公园编号(这个在旅行社订单编辑的时候需要)
        /// </summary>    
        [Required(ErrorMessage = "预订公园为必填项")]
        [DisplayName("公园：")]
        public int ParkId { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>    
        [Required(ErrorMessage = "可订票类为必填项")]
        [DisplayName("可订票类：")]
        public int AgencySaleTicketClassId { get; set; }
        
        /// <summary>
        /// 票数量
        /// </summary>    
        [Required]
        [Range(0, int.MaxValue)]
        public int Qty { get; set; }

        ///// <summary>
        ///// 客户编号
        ///// </summary>    
        //public int? CustomerId { get; set; }
    }
}
