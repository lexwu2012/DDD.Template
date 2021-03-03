using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.Authorization.Users;
using ThemePark.Infrastructure.CustomAttribute;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// AddAgencyInput
    /// </summary>
    [AutoMapTo(typeof(Core.Agencies.Agency))]
    public class AddAgencyInput
    {
        /// <summary>
        /// 代理商名称
        /// </summary>    
        [Required]
        [DisplayName("代理商名称")]
        [StringLength(50)]
        public string AgencyName { get; set; }

        /// <summary>
        /// 代理商类型
        /// </summary>
        [Required]
        public int AgencyTypeId { get; set; }

        public string AgencyTypeName { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>  
        [StringLength(50)]
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "请输入正确的邮箱格式")]
        public string Email { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>    
        [StringLength(20)]
        public string Mobile { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        [DisplayName("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the province identifier.
        /// </summary>
        /// <value>The province identifier.</value>
        [Required, Range(1, long.MaxValue)]
        public long ProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the city identifier.
        /// </summary>
        /// <value>The city identifier.</value>
        [Required, Range(1, long.MaxValue)]
        public long CityId { get; set; }

        /// <summary>
        /// 账户是否启用
        /// </summary>
        public bool IsActive { get; set; }

        ///// <summary>
        ///// 是否启用
        ///// </summary>
        //public bool IsActive { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        //[RequiredIf("AgencyTypeName",nameof(DefaultAgencyType.Ota))]
        [StringLength(User.MaxPasswordLength, MinimumLength = User.MinPasswordLength)]
        public string Password { get; set; }

        ///// <summary>
        ///// 重复新密码
        ///// </summary>
        //[Required]
        //[StringLength(User.MaxPasswordLength, MinimumLength = User.MinPasswordLength)]
        //[Compare("Password")]
        //[DataType(DataType.Password)]
        //public string ConfirmPwd { get; set; }

        ///// <summary>
        ///// 登录名(默认和旅行社名称一样)，唯一
        ///// </summary>
        //[Required, StringLength(User.MaxUserNameLength)]
        //public string UserName { get; set; }

        ///// <summary>
        ///// 旅行社名称
        ///// </summary>
        //[Required, StringLength(User.MaxNameLength)]
        //[DisplayName("姓名")]
        //public string Name { get; set; }

        ///// <summary>
        ///// 许可主机IP
        ///// </summary>
        //[Required]
        //public string HostIPs { get; set; }


        #region AgencyUser(可选)

        /// <summary>
        /// AgencyUser
        /// </summary>
        public AddAgencyUserInput AddAgencyUserInput { get; set; }

        #endregion AgencyUser
    }
}