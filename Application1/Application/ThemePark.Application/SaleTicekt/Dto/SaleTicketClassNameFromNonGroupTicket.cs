using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapFrom(typeof(NonGroupTicket))]
    public class SaleTicketClassNameFromNonGroupTicket
    {
        /// <summary>
        /// 促销票类名称
        /// </summary>
        [MapFrom(nameof(NonGroupTicket.ParkSaleTicketClass),nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public string SaleTicketClassName { get; set; }
    }
}
