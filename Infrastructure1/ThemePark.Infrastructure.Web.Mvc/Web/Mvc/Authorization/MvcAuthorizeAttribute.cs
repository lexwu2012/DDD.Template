using System.Web.Mvc;
using Abp.Authorization;

namespace ThemePark.Infrastructure.Web.Mvc.Authorization
{
    /// <summary>
    /// 权限验证
    /// </summary>
    public class MvcAuthorizeAttribute : AuthorizeAttribute, IAbpAuthorizeAttribute
    {
        /// <summary>
        /// 验证的权限
        /// </summary>
        public string[] Permissions { get; }

        /// <summary>
        /// 是否需要所有权限
        /// </summary>
        public bool RequireAllPermissions { get; set; }

        /// <summary>
        /// 验证权限
        /// </summary>
        public MvcAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        public MvcAuthorizeAttribute(bool requireAllPermissions, params string[] permissions)
        {
            RequireAllPermissions = requireAllPermissions;
            Permissions = permissions;
        }
    }
}
