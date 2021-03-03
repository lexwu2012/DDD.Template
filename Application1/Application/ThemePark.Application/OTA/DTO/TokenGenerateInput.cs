using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 获取Token令牌参数
    /// </summary>
    public class TokenGenerateInput
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string ft_api_name { get; set; }

        /// <summary>
        /// 密码
        /// 注:将给到的明文密码进行32位md5加密
        /// </summary>
        [Required]
        public string ft_api_password { get; set; }
    }

   
}
