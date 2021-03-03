using System;

namespace ThemePark.Application.Message.Dto
{
    /// <summary>
    /// 发送短信Dto
    /// </summary>
    public class MessageSendDto
    {
        /// <summary>
        /// apiToken
        /// </summary>
        public string ApiToken { get; set; }
        /// <summary>
        /// 接收用户手机号码，多个手机号码用英文逗号,分隔，最多500个
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string SmsContent { get; set; }
        /// <summary>
        /// 短信类型
        /// </summary>
        public SmsType SmsType { get; set; }
        /// <summary>
        /// 签名类型
        /// </summary>
        public SignType SignType { get; set; }
    }

    /// <summary>
    /// 短信类型
    /// </summary>
    public enum SmsType
    {
        /// <summary>
        /// 行业：用于发送验证码、行业通知等
        /// </summary>
        Notice = 1,

        /// <summary>
        /// 营销：用于发送营销活动推广消息
        /// </summary>
        Marketing = 2
    }

    /// <summary>
    /// 签名类型
    /// </summary>
    public enum SignType
    {
        /// <summary>
        /// 【华强方特】
        /// </summary>
        HQFangte = 1,

        /// <summary>
        /// 【方特旅游】
        /// </summary>
        FangteTravel = 2
    }
}
