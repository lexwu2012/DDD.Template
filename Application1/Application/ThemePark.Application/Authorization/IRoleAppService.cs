using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.ApplicationDto.Authorization;
using ThemePark.Core.Authorization.Roles;
using ThemePark.Core.Authorization.Users;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Authorization
{
    public interface IRoleAppService : IApplicationService
    {
        /// <summary>
        /// 更新角色的权限
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task.</returns>
        Task UpdateRolePermissions(UpdateRolePermissionsInput input);

        /// <summary>
        /// 获取用户角色名
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <returns>Task&lt;Dictionary&lt;System.Int64, System.String&gt;&gt;.</returns>
        Task<Dictionary<long, List<string>>> GetUsersRoleNamesAsync(IList<long> userIds);

        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        Task<List<TDto>> GetUserRoleListAsync<TDto>(long userId);

        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="userIds">The user ids.</param>
        /// <returns>Task&lt;List&lt;OnlyRoleDto&gt;&gt;.</returns>
        Task<List<TDto>> GetUserRolesAsync<TDto>(IList<long> userIds);

        /// <summary>
        /// 获取用户的所有角色和权限
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        Task<List<TDto>> GetUserRolesAndPermissionsAsync<TDto>(long userId);

        /// <summary>
        /// 获取所有角色信息
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        Task<List<TDto>> GetAllRolesAsync<TDto>();

        /// <summary>
        /// 获取所有角色和权限
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        Task<List<TDto>> GetAllRolesAndPermissionsAsync<TDto>();

        /// <summary>
        /// 获取符合条件的角色和权限
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        Task<List<TDto>> GetAllRolesAndPermissionsAsync<TDto>(Expression<Func<Role, bool>> predicate);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> CreateRoleAsync(CreateRoleInput input);

        /// <summary>
        /// 获取指定的角色和权限
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Role&gt;.</returns>
        Task<RoleDto> GetRoleAndPermissionAsync(int id);

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateRoleAsync(UpdateRoleInput input);

        /// <summary>
        /// 根据角色查询用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<IQueryable<User>> GetUsersByRole(int roleId);
    }
}
