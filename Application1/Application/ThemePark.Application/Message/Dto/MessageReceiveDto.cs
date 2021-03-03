using System;

namespace ThemePark.Application.Message.Dto
{
    /// <summary>
    /// 发送短信返回结果
    /// </summary>
    public class MessageReceiveDto
    {
        /// <summary>
        /// 返回状态（1是成功）
        /// </summary>
        public int ResultStatus { get; set; }
        /// <summary>
        /// 发送状态(0是成功)
        /// </summary>
        public int Data { get; set; }
        /// <summary>
        /// 返回消息描述
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
