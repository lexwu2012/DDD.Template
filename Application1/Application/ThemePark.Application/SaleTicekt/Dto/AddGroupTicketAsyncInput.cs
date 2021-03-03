using System.Collections.Generic;

namespace ThemePark.Application.SaleTicekt.Dto
{
    public class AddGroupTicketInput
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
        /// 订单号，非空的时候为旅行社订单
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商类型ID
        /// </summary>
        public int AgencyTypeId { get; set; }

    }
}
