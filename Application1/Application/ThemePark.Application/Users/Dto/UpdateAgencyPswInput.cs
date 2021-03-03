using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Users.Dto
{
    public class UpdateAgencyPswInput
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required]
        [StringLength(12, MinimumLength = 6)]
        public string OldPassword { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(12, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
