using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.Users.Dto;
using ThemePark.ApplicationDto.Users;
using ThemePark.Core.Agencies;
using ThemePark.Core.Authorization.Users;
using ThemePark.Core.Users;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Users
{
    /// <summary>
    /// Interface IUserAppService
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService"/>
    public interface IUserAppService : IApplicationService, IUserInfoService
    {
        #region Methods

        /// <summary>
        /// 新增系统用户
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task.</returns>
        Task<Result> AddSysUserAsync(CreateUserInput input);

        /// <summary>
        /// 添加用户喜好菜单列表
        /// </summary>
        /// <returns></returns>
        Task<Result> AddUserPreferMenuAsnyc(string menuName, string permissionName);

        /// <summary>
        /// 删除用户喜好菜单列表
        /// </summary>
        /// <returns></returns>
        Task<Result> DeleteUserPreferMenuAsnyc(string entityName, string permissionName);

        /// <summary>
        /// 获取代理商用户信息
        /// </summary>
        Task<TDto> GetAgencyUserAsync<TDto>(IQuery<AgencyUser> query);

        /// <summary>
        /// 根据Id获取系统用户
        /// </summary>
        /// <typeparam name="TDto">The type of the return.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TDto&gt;.</returns>
        Task<TDto> GetSysUserByIdAsync<TDto>(long id);

        /// <summary>
        /// 删除系统用户
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> DeleteSysUserAsync(long id);

        /// <summary>
        /// 删除代理商用户
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> DeleteAgencyUserAsync(long id);

        /// <summary>
        /// 系统用户分页查询方法
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetSysUsersByPageAsync<TDto>(IPageQuery<SysUser> pageQuery);

        /// <summary>
        /// 系统用户分页查询方法，根据数据权限过滤，只显示当前用户同级及下级或者未设置数据权限的用户
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetDataPermissionSysUsersByPageAsync<TDto>(IPageQuery<SysUser> pageQuery);

        /// <summary>
        /// 所有用户分页查询方法
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetAllUsersByPageAsync<TDto>(IPageQuery<User> pageQuery);

        /// <summary>
        /// 根据条件获取用户喜好菜单列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetUserPreferMenusListAsnyc<TDto>(IQuery<UserPreferMenu> query);
        
        /// <summary>
        /// Removes from role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Task.</returns>
        Task RemoveFromRole(long userId, string roleName);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdatePasswordAsync(UpdatePasswordInput input);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> ResetPasswordAsync(ResetPasswordInput input);

        /// <summary>
        /// 修改手机号
        /// </summary>
        Task<Result> UpdatePhoneByOldPhoneAsync(long userId, UpdatePhoneInput input);

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateUserAsync(ApplicationDto.Users.UpdateUserInput input);

        /// <summary>
        /// 修改用户数据权限
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateUserDataPermissionAsync(UpdateUserDataPermissionInput input);

        /// <summary>
        /// 修改用户角色
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateUserRolesAsync(UpdateUserRoleInput input);

        /// <summary>
        /// 接收客户端用户修改密码（数据同步）
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdatePasswordDatasync(ChangePasswordDto input);

        #endregion Methods
    }
}