using System;
using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 支付订单查询结果
    /// </summary>
    public class PaymentOrderQueryResponse
    {
        /// <summary>
        /// 系统交易号
        /// </summary>
        [Required, StringLength(64)]
        public string TradeNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        /// <example>HQ20151015649584</example>
        [Required]
        [StringLength(64)]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public PaymentOrderStatus Status { get; set; }

        /// <summary>
        /// 交易状态提示
        /// </summary>
        [Display(Name = "交易状态提示")]
        [StringLength(128)]
        public string StatusMsg { get; set; }

        /// <summary>
        /// 订单金额（元），精确到 0.01
        /// </summary>
        /// <example>23.50</example>
        [Required]
        [Range(typeof(decimal), "0.01", "100000000.0")]
        public decimal OrderFee { get; set; }

        /// <summary>
        /// 付款金额（元），精确到 0.01
        /// </summary>
        /// <example>23.50</example>
        [Range(typeof(decimal), "0.01", "100000000.0")]
        public decimal? PaidFee { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        /// <remarks>
        /// yyyy-MM-dd HH:mm:ss
        /// </remarks>
        public DateTime? PaidTime { get; set; }

        /// <summary>
        /// 支付平台类型
        /// </summary>
        [Display(Name = "支付平台")]
        public PaymentPayPlatform? Platform { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentPayMode? PayMode { get; set; }

        /// <summary>
        /// 支付订单号
        /// </summary>
        /// <remarks>
        /// 支付平台交易流水号（支付宝、微信...）
        /// </remarks>
        [Display(Name = "支付订单号")]
        [StringLength(64)]
        public string PayOrderNo { get; set; }

        /// <summary>
        /// 附加参数
        /// </summary>
        /// <remarks>如果用户下单时传递了该参数，则返回给商户时会回传该参数。</remarks>
        /// <example>{"Info":"test"}</example>
        [StringLength(1024)]
        public string OutAttachData { get; set; }
    }
}
