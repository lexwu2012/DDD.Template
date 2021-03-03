using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum PaymentOrderStatus
    {
        /// <summary>
        /// 支付失败
        /// </summary>
        [Display(Name = "支付失败")]
        PlayError = -10,

        /// <summary>
        /// 未支付
        /// </summary>
        [Display(Name = "未支付")]
        NotPay = 0,

        /// <summary>
        /// 支付中
        /// </summary>
        [Display(Name = "支付中")]
        Paying = 5,

        /// <summary>
        /// 已关闭
        /// </summary>
        [Display(Name = "已关闭")]
        Closed = 10,

        /// <summary>
        /// 支付成功
        /// </summary>
        [Display(Name = "支付成功")]
        Success = 20,

        /// <summary>
        /// 转入退款
        /// </summary>
        [Display(Name = "转入退款")]
        Refund = 30,
    }
}