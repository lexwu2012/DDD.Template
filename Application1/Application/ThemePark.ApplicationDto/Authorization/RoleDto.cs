using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Authorization.Roles;

namespace ThemePark.ApplicationDto.Authorization
{
    /// <summary>
    /// 角色 完整DTO对象
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.FullAuditedEntityDto" />
    [AutoMapFrom(typeof(Role))]
    public class RoleDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 角色
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否为固定角色
        /// 固定角色不能删除，不能修改角色名称。
        /// 固定角色用于系统内部使用。
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 角色授权设置
        /// </summary>
        public IList<RolePermissionSetting> Permissions { get; set; }
    }
}
