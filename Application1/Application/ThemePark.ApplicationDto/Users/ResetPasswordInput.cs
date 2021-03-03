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
    public class ResetPasswordInput : EntityDto<long>
    {
        #region Properties
        
        /// <summary>
        /// 密码
        /// </summary>
        [DisplayName("密码")]
        [Required]
        [StringLength(User.MaxPasswordLength, MinimumLength = User.MinPasswordLength)]
        public string Password { get; set; }

        #endregion Properties
    }
}