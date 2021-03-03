using System;
using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// Token令牌参数
    /// </summary>
    public class TokenGenerateOutput
    {
        /// <summary>
        /// 访问接口令牌
        /// </summary>
        [Required]
        public string token { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        [Required]
        public DateTime expiredTime { get; set; }
    }
}
