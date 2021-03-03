using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 统一下单业务参数
    /// </summary>
    public class PaymentUnifedOrderData
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [Required]
        [StringLength(64)]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 结果通知地址
        /// </summary>
        [StringLength(512)]
        public string OutNotifyUrl { get; set; }

        /// <summary>
        /// 附加参数
        /// </summary>
        /// <remarks>如果用户请求时传递了该参数，则返回给商户时会回传该参数。</remarks>
        /// <example>{"Info":"test"}</example>
        [StringLength(1024)]
        public string OutAttachData { get; set; }

        /// <summary>
        /// 订单金额（元），精确到 0.01
        /// </summary>
        /// <example>23.50</example>
        [Required]
        [Range(typeof(decimal), "0.01", "100000000.0")]
        public decimal OrderFee { get; set; }

        /// <summary>
        /// 交易说明
        /// </summary>
        /// <example>Ipad mini  16G  白色</example>
        [Required]
        [StringLength(256)]
        public string OutOrderDesc { get; set; }

        /// <summary>
        /// 交易详细说明
        /// </summary>
        /// <example>Ipad mini  16G  白色</example>
        [StringLength(4000)]
        public string OutOrderDetail { get; set; }

        /// <summary>
        /// 发起付款的客户端Ip
        /// </summary>
        [StringLength(64)]
        public string ClientIp { get; set; }
    }
}
