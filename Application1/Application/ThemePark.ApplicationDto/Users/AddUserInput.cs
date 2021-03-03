using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.ApplicationDto.Users
{
    /// <summary>
    /// 添加用户
    /// </summary>
    [AutoMapTo(typeof(User))]
    public class AddUserInput
    {
        #region Properties

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [StringLength(User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 邮箱是否已确认
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// 电话号码是否已确认
        /// </summary>
        public bool IsPhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(User.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(User.MaxPasswordLength, MinimumLength = User.MinPasswordLength)]
        public string Password { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [StringLength(User.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 登录名，唯一
        /// </summary>
        [StringLength(User.MaxUserNameLength)]
        public string UserName { get; set; }


        #endregion Properties
    }
}