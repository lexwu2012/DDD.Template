using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapTo(typeof(VipCardRenewalSet))]
    public class YearCardRenewalSetInput
    {
        /// <summary>
        /// 基础票类Id
        /// </summary>
        [Required]
        public int TicketClassId { get; set; }

        /// <summary>
        /// 首次续卡价格
        /// </summary>
        [Required]
        public decimal FirstPrice { get; set; }

        /// <summary>
        /// 多次续卡价格
        /// </summary>
        [Required]
        public decimal MulPrice { get; set; }

    }
}
