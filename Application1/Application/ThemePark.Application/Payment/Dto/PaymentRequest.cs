using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 签名输入参数
    /// </summary>
    [JsonConverter(typeof(PaymentDataJsonConverter))]
    public class PaymentRequest<TData> : PaymentRequest
    {
        /// <summary>
        /// 业务参数
        /// </summary>
        /// <remarks>
        /// JSON字符串格式
        /// </remarks>
        /// <example>"{OutOrderNO:'HT123456789', OrderFee: 0.1}"</example>
        [Required]
        public TData Data
        {
            get { return this.GetJsonValue(default(TData)); }
            set { this.SetJsonValue(value); }
        }
    }

    /// <summary>
    /// 签名输入参数
    /// </summary>
    [JsonConverter(typeof(PaymentDataJsonConverter))]
    public class PaymentRequest : PaymentData
    {
        /// <summary>
        /// 合作平台身份ID
        /// </summary>
        [Required]
        public string Partner
        {
            get { return this.GetValue(nameof(Partner)); }
            set { this.SetValue(nameof(Partner), value); }
        }

        /// <summary>
        /// 时间搓
        /// </summary>
        [Required]
        public DateTime? TimeStamp
        {
            get { return this.GetValue(nameof(TimeStamp), new DateTime?(), DateFormat); }
            set { this.SetValue(nameof(TimeStamp), value, DateFormat); }
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
