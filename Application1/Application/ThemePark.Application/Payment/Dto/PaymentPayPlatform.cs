using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 支付平台
    /// </summary>
    public enum PaymentPayPlatform
    {
        /// <summary>
        /// 微信支付
        /// </summary>
        [Display(Name = "微信支付")]
        WxPay = 1,

        /// <summary>
        /// 支付宝
        /// </summary>
        [Display(Name = "支付宝")]
        Alipay = 2,

        /// <summary>
        /// 测试支付
        /// </summary>
        [Display(Name = "测试支付")]
        Test = 1000,
    }
}
