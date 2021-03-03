using System.ComponentModel;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.ApplicationDto.Users
{
    /// <summary>
    /// 修改用户信息
    /// </summary>
    [AutoMapTo(typeof(User))]
    public class UpdateUserInput : EntityDto<long>
    {
        #region Properties
    
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [DisplayName("邮箱地址")]
        [StringLength(User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [DisplayName("是否启用")]
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
        [Required, StringLength(User.MaxNameLength)]
        [DisplayName("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [StringLength(User.MaxPhoneNumberLength)]
        [DisplayName("电话号码")]
        public string PhoneNumber { get; set; }

        #endregion Properties
    }
}