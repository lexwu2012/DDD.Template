using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapTo(typeof(VendorSet))]
    public class AddVendorSetInput
    {
        /// <summary>
        /// 售票机账号
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// 最大可售票数
        /// </summary>
        [Required]
        public int TicketMax { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        [Required]
        public int TerminalId { get; set; }
    }
}
