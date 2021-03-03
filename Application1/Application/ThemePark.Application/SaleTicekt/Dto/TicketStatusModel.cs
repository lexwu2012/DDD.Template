using Abp.AutoMapper;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMap(typeof(NonGroupTicket),typeof(GroupTicket))]
    public class TicketStatusModel
    {
        /// <summary>
        /// 条形码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TicketSaleStatus State { get; set; }

        /// <summary>
        /// 退款Id
        /// </summary>
        public string TicketRefundId { get; set; }

    }
}
