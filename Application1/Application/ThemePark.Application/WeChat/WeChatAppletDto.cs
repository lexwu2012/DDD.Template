using System;
using ThemePark.Core.CoreCache.CacheItem;

namespace ThemePark.Application.WeChat
{
    /// <summary>
    /// 微信用户数据
    /// </summary>
    public class WeChatAppletDto
    {
        public WeChatAppletDto()
        {
            Token = Guid.NewGuid().ToString("N");
        }

        public WeChatAppletDto(AppletCacheItem item) : this()
        {
            OpenId = item.OpenId;
            SessionKey = item.SessionKey;
        }

        /// <summary>
        /// 微信用户Id
        /// </summary>
        /// <value>The open identifier.</value>
        public string OpenId { get; set; }

        /// <summary>
        /// 微信加密密钥
        /// </summary>
        /// <value>The session key.</value>
        public string SessionKey { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; set; }
    }
}
