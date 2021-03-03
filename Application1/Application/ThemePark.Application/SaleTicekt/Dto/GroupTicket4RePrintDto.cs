using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapFrom(typeof(GroupTicket))]
    public class GroupTicket4RePrintDto
    {
        /// <summary>
        /// 基础票类名称
        /// </summary>
        [MapFrom(nameof(GroupTicket.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassTemplate), nameof(AgencySaleTicketClassTemplate.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public string SaleTicketName { get; set; }

    }
}
