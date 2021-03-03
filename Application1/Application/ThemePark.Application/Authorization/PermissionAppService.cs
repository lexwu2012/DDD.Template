using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Authorization;

namespace ThemePark.Application.Authorization
{
    public class PermissionAppService : ThemeParkAppServiceBase, IPermissionAppService
    {
        /// <summary>
        /// The _permission manager
        /// </summary>
        private readonly IPermissionManager _permissionManager;

        /// <summary>
        /// 
        /// </summary>
        public PermissionAppService(IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        /// <summary>
        /// Gets <see cref="T:Abp.Authorization.Permission" /> object with given <paramref name="name" /> or throws exception
        /// if there is no permission with given <paramref name="name" />.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        public Permission GetPermission(string name)
        {
            return _permissionManager.GetPermission(name);
        }

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        /// <returns>IReadOnlyList&lt;Permission&gt;.</returns>
        public IReadOnlyList<Permission> GetAllTopLevelPermissions()
        {
            var all = _permissionManager.GetAllPermissions();

            return GetLevelPermissions(all, 0);
        }

        /// <summary>
        /// Gets the level permissions.
        /// </summary>
        /// <param name="permissions">The permissions.</param>
        /// <param name="level">The level. 0 is system level</param>
        /// <returns>IReadOnlyList&lt;Permission&gt;.</returns>
        private IReadOnlyList<Permission> GetLevelPermissions(IReadOnlyList<Permission> permissions, int level)
        {
            return permissions.Where(o =>
            {
                var parent = o.Parent;
                while (level-- > 0)
                {
                    if (parent == null)
                    {
                        return false;
                    }

                    parent = parent.Parent;
                }

                if (parent == null)
                {
                    return true;
                }

                return false;
            }).ToImmutableList();
        }
    }
}
