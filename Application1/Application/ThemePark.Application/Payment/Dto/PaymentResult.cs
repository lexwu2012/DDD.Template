using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 签名返回结果
    /// </summary>
    [JsonConverter(typeof(PaymentDataJsonConverter))]
    public class PaymentResult<TData> : PaymentResult
    {
        /// <summary>
        /// 业务参数
        /// </summary>
        /// <remarks>
        /// JSON字符串格式
        /// </remarks>
        /// <example>"{TradeNO:'HT123456789'}"</example>
        [Required]
        public TData Data
        {
            get { return this.GetJsonValue(default(TData)); }
            set { this.SetJsonValue(value); }
        }
    }

    /// <summary>
    /// 签名返回结果
    /// </summary>
    [JsonConverter(typeof(PaymentDataJsonConverter))]
    public class PaymentResult : PaymentData
    {
        /// <summary>
        /// 接口调用是否成功，并不表明业务处理结果。
        /// </summary>
        [Required]
        public PaymentResultCode Code
        {
            get { return this.GetValue(nameof(Code), PaymentResultCode.Fail); }
            set { this.SetValue(nameof(Code), value.ToString("D")); }
        }

        /// <summary>
        /// 返回信息，如非空，为错误原因
        /// </summary>
        /// <remarks>
        /// 签名失败
        /// 参数格式校验错误
        /// </remarks>
        public string Message
        {
            get { return this.GetValue(nameof(Message)); }
            set { this.SetValue(nameof(Message), value); }
        }

        /// <summary>
        /// 参数签名值
        /// </summary>
        [Required]
        public string Sign
        {
            get { return this.GetValue(nameof(Sign)); }
            set { this.SetValue(nameof(Sign), value); }
        }
    }
}