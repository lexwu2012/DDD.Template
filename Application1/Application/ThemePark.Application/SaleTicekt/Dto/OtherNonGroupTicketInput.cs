using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapTo(typeof(OtherNonGroupTicket))]
    public class OtherNonGroupTicketInput
    {
        /// <summary>
        /// 门票类型编号
        /// </summary>    
        [Required]
        public int ParkSaleTicketClassId { get; set; }


        /// <summary>
        /// 门票数量
        /// </summary>    
        [Required]
        public int Qty { get; set; }

        /// <summary>
        /// 计划开始使用日期
        /// </summary>    
        [Required]
        public System.DateTime ValidStartDate { get; set; }

    }
}
