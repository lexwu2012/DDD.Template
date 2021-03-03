using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Authorization;

namespace ThemePark.Application.Authorization
{
    public interface IPermissionAppService : IApplicationService
    {
        /// <summary>
        /// Gets <see cref="T:Abp.Authorization.Permission" /> object with given <paramref name="name" /> or throws exception
        /// if there is no permission with given <paramref name="name" />.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        Permission GetPermission(string name);

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        /// <returns>IReadOnlyList&lt;Permission&gt;.</returns>
        IReadOnlyList<Permission> GetAllTopLevelPermissions();
    }
}