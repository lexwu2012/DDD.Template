using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.WeChat
{
    /// <summary>
    /// 微信账号相关信息
    /// </summary>
    public class GetOpenIdInput
    {
        /// <summary>
        /// 小程序唯一标识
        /// </summary>
        /// <value>The application identifier.</value>
        [Required]
        public string AppId { get; set; }

        /// <summary>
        /// 小程序调用接口后返回的服务端通信令牌
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; set; }

        /// <summary>
        /// 登录时获取的 code
        /// </summary>
        /// <value>The code.</value>
        [Required]
        public string Code { get; set; }
    }
}
