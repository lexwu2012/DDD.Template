using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThemePark.ApplicationDto.Users
{
    /// <summary>
    /// 修改用户角色
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.EntityDto{System.Int64}"/>
    public class UpdateUserRoleInput : EntityDto<long>
    {
        #region Properties

        /// <summary>
        /// 用户所有角色
        /// </summary>
        /// <value>The roles.</value>
        [Required]
        public IList<int> Roles { get; set; }

        #endregion Properties
    }
}