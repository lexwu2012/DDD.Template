using Abp.AutoMapper;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 与公园相关dto
    /// </summary>
    [AutoMapFrom(typeof(ParkSaleTicketClass))]
    public class ParkSaleTicketClass4ParkDto
    {
        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(ParkSaleTicketClass.Park),nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 是否平日
        /// </summary>
        public bool? IsEveryday { get; set; }

        /// <summary>
        /// 开始销售时间
        /// </summary>
        public System.DateTime? SaleStartDate { get; set; }

        /// <summary>
        /// 销售结束时间
        /// </summary>
        public System.DateTime? SaleEndDate { get; set; }
    }
}
