using System.Collections.Generic;
using ThemePark.Application.SaleTicekt.Dto;

namespace ThemePark.Application.AgentTicket.Dto
{
    public class AddTravelTicketInput
    {
        /// <summary>
        /// 购票信息
        /// </summary>
        public List<GroupTicketInput> GroupInputs { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        public InvoiceInput InvoiceInput { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string Tradeno { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
    }
}
