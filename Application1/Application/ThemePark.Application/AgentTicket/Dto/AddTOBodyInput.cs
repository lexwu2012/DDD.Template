using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 添加旅行社预订订单体
    /// </summary>
    [AutoMapTo(typeof(TOBodyPre))]
    public class TravelTOBodyInput
    {
        /// <summary>
        /// 公园编号
        /// </summary>    
        [Required(ErrorMessage ="预订公园为必填项")]
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
        [Range(1, int.MaxValue)]
        public int Qty { get; set; }
        
        ///// <summary>
        ///// 顾客信息
        ///// </summary>
        //public CustomerInput Customer { get; set; }

        ///// <summary>
        ///// 备注
        ///// </summary>    
        //[StringLength(50,ErrorMessage ="请小于50字")]
        //public string Remark { get; set; }

    }
}
