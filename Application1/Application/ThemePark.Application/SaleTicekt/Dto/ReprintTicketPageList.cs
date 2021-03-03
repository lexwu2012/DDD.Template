using ThemePark.Application.SaleTicekt.Interfaces;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 重打印页面门票列表
    /// </summary>
    public class ReprintTicketPageRecord
    {
        /// <summary>
        /// 票类名称
        /// </summary>
        public string SaleTicketName { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 打印列表类型
        /// </summary>
        public PrintTicketType PrintTicketType { get; set; }
    }
}
