using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapTo(typeof(VendorSet))]
    public class UpdateVendorSetInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 最大可售票数
        /// </summary>
        public int TicketMax { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public int TerminalId { get; set; }

        /// <summary>
        /// 售票机账号
        /// </summary>
        public long UserId { get; set; }

    }
}
