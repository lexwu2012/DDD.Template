using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PaymentPayMode
    {
        /// <summary>
        /// Web浏览器
        /// </summary>
        [Display(Name = "Web浏览器")]
        WebBrowser = 1,

        /// <summary>
        /// 原生APP
        /// </summary>
        [Display(Name = "原生APP")]
        NativeApp = 2,

        /// <summary>
        /// 商户扫码
        /// </summary>
        [Display(Name = "商户扫码")]
        PartnerScanCode = 3,
        
        /// <summary>
        /// 微信公众号
        /// </summary>
        [Display(Name = "微信公众号")]
        WeixinJsapi = 101,
    }
}