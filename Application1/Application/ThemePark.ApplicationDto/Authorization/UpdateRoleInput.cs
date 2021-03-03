using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Authorization.Roles;

namespace ThemePark.ApplicationDto.Authorization
{
    [AutoMapTo(typeof(Role))]
    public class UpdateRoleInput
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Required, StringLength(Role.MaxNameLength)]
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [Required, StringLength(Role.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 角色授权设置
        /// </summary>
        public ICollection<string> Permissions { get; set; }

        #endregion Properties
    }
}