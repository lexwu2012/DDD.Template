using System;

namespace ThemePark.Application.Message.Dto
{
    /// <summary>
    /// 短信接口登录Dto
    /// </summary>
    public class MessageTokenDto
    {
        /// <summary>
        /// 返回状态（1是成功）
        /// </summary>
        public int ResultStatus { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public Token Data { get; set; }
        /// <summary>
        /// 返回消息描述
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回时间
        /// </summary>
        public DateTime Time { get; set; }
    }

    /// <summary>
    /// Token信息
    /// </summary>
    public class Token
    {
        /// <summary>
        /// apiToken
        /// </summary>
        public string ApiToken { get; set; }

        /// <summary>
        /// ownToken
        /// </summary>
        public string OwnToken { get; set; }
    }
}
