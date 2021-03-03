using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 退票输入
    /// </summary>
    public class RefundTicketInput
    {
        /// <summary>
        /// 条形码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 分票总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 门票类型
        /// </summary>
        public TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// 票类型（散客票，团体票）
        /// </summary>
        public string TicketCatogory { get; set; }
    }
}
