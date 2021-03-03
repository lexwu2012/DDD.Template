using System.Collections.Generic;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.Trade.Dto;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡凭证销售信息
    /// </summary>
    public class SaveYearCardVoucherDto
    {
        /// <summary>
        /// 票类详情
        /// </summary>
        public List<VIPVoucherInput> VoucherInfos { get; set; }

        /// <summary>
        /// 支付详情
        /// </summary>
        public TradeInfoInput TradeInfos { get; set; }

        /// <summary>
        /// 输入的发票信息
        /// </summary>
        public InvoiceInput invoiceInput { get; set; }
    }
}
