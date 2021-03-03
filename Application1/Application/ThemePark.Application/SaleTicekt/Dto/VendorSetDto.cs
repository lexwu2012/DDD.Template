using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 自助售票机发票配置
    /// </summary>
    [AutoMapFrom(typeof(VendorSet))]
    public class VendorSetDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 最大可售票数
        /// </summary>
        public int TicketMax { get; set; }
    }
}
