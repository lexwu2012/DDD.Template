using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡销售数据
    /// </summary>
    [AutoMap(typeof(VIPCard))]
    public class VipCardSaleInfo
    {
        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 基础票类Id
        /// </summary>
        [MapFrom(nameof(VIPCard.TicketClass),nameof(TicketClass.TicketClassName))]
        public string TicketClassName { get; set; }
    }
}
