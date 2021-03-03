using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.ApplicationDto.Users
{
    /// <summary>
    /// 修改用户信息
    /// </summary>
    /// <seealso cref="long"/>
    [AutoMapTo(typeof(User))]
    public class UpdatePasswordInput : EntityDto<long>
    {
        #region Properties

        /// <summary>
        /// 旧密码
        /// </summary>
        [DisplayName("旧密码")]
        [Required]
        [StringLength(User.MaxPasswordLength, MinimumLength = User.MinPasswordLength)]
        public string OldPassword { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DisplayName("新密码")]
        [Required]
        [StringLength(User.MaxPasswordLength, MinimumLength = User.MinPasswordLength)]
        public string Password { get; set; }

        #endregion Properties
    }
}