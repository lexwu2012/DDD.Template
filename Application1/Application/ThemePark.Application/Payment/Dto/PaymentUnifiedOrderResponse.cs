using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 统一下单返回业务参数
    /// </summary>
    public class PaymentUnifiedOrderResponse
    {
        /// <summary>
        /// 系统交易号
        /// </summary>
        [Required, StringLength(64)]
        public string TradeNo { get; set; }
    }
}