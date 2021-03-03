using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 商户扫码支付业务参数
    /// </summary>
    public class PaymentPartnerScanCodePayData
    {
        /// <summary>
        /// 统一下单返回的交易号
        /// </summary>
        /// <remarks>
        /// UnifiedOrder 和 TradeNo 两者选其一，UnifiedOrder 优先
        /// </remarks>
        [StringLength(64)]
        public string TradeNo { get; set; }

        /// <summary>
        /// 统一下单业务参数
        /// </summary>
        /// <remarks>
        /// UnifiedOrder 和 OutOrderNo 两者选其一，UnifiedOrder 优先
        /// </remarks>
        public PaymentUnifedOrderData UnifiedOrder { get; set; }

        /// <summary>
        /// 支付平台
        /// </summary>
        [Required]
        public PaymentPayPlatform? Platform { get; set; }

        /// <summary>
        /// 支付授权码
        /// </summary>
        /// <remarks>
        /// 通过扫码设备得到用户展示的支付授权码
        /// </remarks>
        [Required(AllowEmptyStrings = false)]
        [DisplayName("支付授权码")]
        public string AuthCode { get; set; }
    }
}
