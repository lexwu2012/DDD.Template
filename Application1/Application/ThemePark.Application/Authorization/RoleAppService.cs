using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.ApplicationDto.Authorization;
using ThemePark.Core.Authorization.Roles;
using ThemePark.Core.Authorization.Users;
using ThemePark.Infrastructure.EntityFramework;
using System;
using System.Data.Entity;
using Abp.Authorization;
using AutoMapper.QueryableExtensions;
using ThemePark.Common;
using ThemePark.Core.BasicData;
using ThemePark.Core.Users;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Authorization
{
    /// <summary>
    /// Class RoleAppService.
    /// </summary>
    /// <seealso cref="ThemePark.Application.ThemeParkAppServiceBase" />
    /// <seealso cref="ThemePark.Application.Authorization.IRoleAppService" />
    public class RoleAppService : ThemeParkAppServiceBase, IRoleAppService
    {
        private readonly RoleManager _roleManager;

        private readonly RoleStore _roleStore;

        private readonly IRepository<UserRole> _userRoleRepository;

        private readonly IRepository<Role> _roleRepository;

        private readonly IPermissionManager _permissionManager;
        private readonly IRepository<SysUser, long> _sysUserRepository;
        private readonly IRepository<ParkArea> _parkAreaRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleAppService"/> class.
        /// </summary>
        /// <param name="roleManager">The role manager.</param>
        /// <param name="userRoleRepository">The user role repository.</param>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="roleStore">The role store.</param>
        /// <param name="permissionManager">The permission manager.</param>
        public RoleAppService(RoleManager roleManager, IRepository<UserRole> userRoleRepository,
            IRepository<Role> roleRepository, RoleStore roleStore, IPermissionManager permissionManager, IRepository<SysUser, long> sysUserreRepository, IRepository<ParkArea> parkAreaRepository)
        {
            _roleManager = roleManager;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _roleStore = roleStore;
            _permissionManager = permissionManager;
            _sysUserRepository = sysUserreRepository;
            _parkAreaRepository = parkAreaRepository;
        }

        /// <summary>
        /// 更新角色的权限
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task.</returns>
        public async Task UpdateRolePermissions(UpdateRolePermissionsInput input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.RoleId);

            await _roleManager.SetGrantedPermissionsAsync(role, input.GrantedPermissionNames);
        }

        /// <summary>
        /// 获取用户角色名
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <returns>Task&lt;Dictionary&lt;System.Int64, System.String&gt;&gt;.</returns>
        public async Task<Dictionary<long, List<string>>> GetUsersRoleNamesAsync(IList<long> userIds)
        {
            //var query = from userRole in _userRoleRepository.GetAll()
            //            join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
            //            where userIds.Contains(userRole.UserId)
            //            select new { userRole.UserId, role.Name };

            var query = _userRoleRepository.GetAll().Where(o => userIds.Contains(o.UserId))
                .Join(_roleRepository.GetAll(), userRole => userRole.RoleId, role => role.Id,
                (userRole, role) => new { userRole.UserId, role.DisplayName });
            var list = await query.ToListAsync();

            var dic = list.GroupBy(o => o.UserId)
                .ToDictionary(o => o.Key, grouping => grouping.Select(o => o.DisplayName).ToList());

            return dic;
        }


        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        public Task<List<TDto>> GetUserRoleListAsync<TDto>(long userId)
        {
            var query = _userRoleRepository.AsNoTrackingAndInclude(o => o.Role)
                .Where(o => o.UserId.CompareTo(userId) == 0)
                .OrderBy(o => o.Id).ProjectTo<TDto>();

            return query.ToListAsync();
        }

        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="userIds">The user ids.</param>
        /// <returns>Task&lt;List&lt;OnlyRoleDto&gt;&gt;.</returns>
        public Task<List<TDto>> GetUserRolesAsync<TDto>(IList<long> userIds)
        {
            var query = _userRoleRepository.AsNoTrackingAndInclude(o => o.Role)
                .Where(o => userIds.Contains(o.UserId))
                .OrderBy(o => o.Id).ProjectTo<TDto>();

            return query.ToListAsync();
        }

        /// <summary>
        /// 获取用户的所有角色和权限
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        public Task<List<TDto>> GetUserRolesAndPermissionsAsync<TDto>(long userId)
        {
            var query = _userRoleRepository.AsNoTrackingAndInclude(o => o.Role, o => o.Role.Permissions)
                .Where(o => o.UserId.CompareTo(userId) == 0).OrderBy(o => o.Id)
                .ProjectTo<TDto>();

            return query.ToListAsync();
        }

        /// <summary>
        /// 获取所有角色信息
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        public Task<List<TDto>> GetAllRolesAsync<TDto>()
        {
            //TODOCuizj: query from store?

            return _roleStore.Roles.OrderBy(o => o.Id)
                    .ProjectTo<TDto>().ToListAsync();
        }

        /// <summary>
        /// 获取所有角色和权限
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        public Task<List<TDto>> GetAllRolesAndPermissionsAsync<TDto>()
        {
            return _roleStore.RolesAndPermissions.OrderBy(o => o.Id)
                    .ProjectTo<TDto>().ToListAsync();
        }

        /// <summary>
        /// 获取符合条件的角色和权限
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <returns>Task&lt;List&lt;TDto&gt;&gt;.</returns>
        public Task<List<TDto>> GetAllRolesAndPermissionsAsync<TDto>(Expression<Func<Role, bool>> predicate)
        {
            return _roleStore.RolesAndPermissions.Where(predicate).OrderBy(o => o.Id)
                    .ProjectTo<TDto>().ToListAsync();
        }

        /// <summary>
        /// 创建角色的同时赋予权限
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> CreateRoleAsync(CreateRoleInput input)
        {
            input.SelectPermissions = input.SelectPermissions.Distinct().Where(o => !string.IsNullOrWhiteSpace(o)).ToList();

            //check permission validate
            foreach (var permission in input.SelectPermissions)
            {
                if (!CheckPermission(permission))
                {
                    return Result.FromError($"错误的权限信息：{permission}");
                }
            }

            var role = input.MapTo<Role>();
            role.Permissions = input.SelectPermissions.Select(o => new RolePermissionSetting() { Name = o }).ToList();

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return Result.FromError(result.Errors.First());
            }

            return Result.Ok();
        }

        /// <summary>
        /// 获取指定的角色和权限
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Role&gt;.</returns>
        public async Task<RoleDto> GetRoleAndPermissionAsync(int id)
        {
            var role = await _roleStore.RolesAndPermissions.Where(o => o.Id == id).FirstAsync();

            return role.MapTo<RoleDto>();
        }

        private bool CheckPermission(string name)
        {
            return _permissionManager.GetPermissionOrNull(name) != null;
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdateRoleAsync(UpdateRoleInput input)
        {
            input.Permissions = input.Permissions.Distinct().Where(o => !string.IsNullOrWhiteSpace(o)).ToList();

            //check permission validate
            foreach (var permission in input.Permissions)
            {
                if (!CheckPermission(permission))
                {
                    return Result.FromError($"错误的权限信息：{permission}");
                }
            }

            //add role
            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            var ownPermis = role.Permissions.Select(o => o.Name).ToList();

            var intersect = ownPermis.Intersect(input.Permissions).ToList();

            //remove permission
            var remove = ownPermis.Except(intersect);
            var rPermis = role.Permissions.Where(o => remove.Contains(o.Name)).ToList();
            rPermis.ForEach(async o =>
            {
                await _roleManager.ProhibitPermissionAsync(role, o.Name);
            });

            //add new permission
            var add = input.Permissions.Except(intersect);
            add.ForEach(async o => await _roleManager.GrantPermissionAsync(role, o));

            //update role
            role.Name = input.Name;
            role.DisplayName = input.DisplayName;
            role.IsActive = input.IsActive;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                Result.FromError(result.Errors.First());
            }

            return Result.Ok();
        }

        /// <summary>
        /// 根据角色查询用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<IQueryable<User>> GetUsersByRole(int roleId)
        {
            //查询登录人的权限
            var userParkAreaId = await _sysUserRepository.GetAll()
                .Where(m => m.Id == AbpSession.UserId)
                .Select(m => m.ParkAreaId)
                .FirstOrDefaultAsync();

            var parkAreaIds = _parkAreaRepository.GetAll().Where(m => m.ParentPath.Contains(ParkArea.Separator + userParkAreaId + ParkArea.Separator))
                .Select(m => m.Id).ToList();

            //需要包含登录人的权限区域
            parkAreaIds.Add(userParkAreaId.Value);

            var parkAreaUserIds = _sysUserRepository.GetAll().Where(p => parkAreaIds.Contains(p.ParkAreaId.Value)).Select(p => p.User.Id);

            //查询角色下的且包含在权限下的用户
            var sysUsers = _userRoleRepository.AsNoTracking().Where(m => m.RoleId == roleId)
               .Select(m => m.User).Where(m => m.UserType == UserType.SysUser && parkAreaUserIds.Contains(m.Id));

            return sysUsers;

        }
    }
}

