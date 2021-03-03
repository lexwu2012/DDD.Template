using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.Users.Dto;
using ThemePark.ApplicationDto.Users;
using ThemePark.Core;
using ThemePark.Core.Agencies;
using ThemePark.Core.Authorization.Users;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Core.Users;
using ThemePark.Infrastructure;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.Users
{
    /// <summary>
    /// Class UserAppService.
    /// </summary>
    /// <seealso cref="ThemePark.Application.ThemeParkAppServiceBase"/>
    /// <seealso cref="ThemePark.Application.Users.IUserAppService"/>
    public class UserAppService : ThemeParkAppServiceBase, IUserAppService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// The _sysuser repository
        /// </summary>
        private readonly IRepository<SysUser, long> _sysUserRepository;

        /// <summary>
        /// The _user manager
        /// </summary>
        private readonly UserManager _userManager;

        private readonly IRepository<UserPreferMenu> _userPreferMenuRepository;

        /// <summary>
        /// The _user repository
        /// </summary>
        private readonly IRepository<User, long> _userRepository;

        private readonly IRepository<AgencyUser, long> _agencyUserRepository;
        
        private readonly IParkAreaRepository _parkAreaRepository;

        /// <summary>
        /// The _user store
        /// </summary>
        private readonly UserStore _userStore;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAppService" /> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="sysUserRepository">The sysuser repository.</param>
        /// <param name="userStore">The user store.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="userPreferMenuRepository">The cache manager.</param>
        /// <param name="apiUserRepository">The API user repository.</param>
        /// <param name="agencyUserRepository"></param>
        public UserAppService(IRepository<User, long> userRepository, UserManager userManager, IRepository<SysUser, long> sysUserRepository,
            UserStore userStore, ICacheManager cacheManager, IRepository<UserPreferMenu> userPreferMenuRepository, 
            IRepository<AgencyUser, long> agencyUserRepository, IParkAreaRepository parkAreaRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _sysUserRepository = sysUserRepository;
            _userStore = userStore;
            _cacheManager = cacheManager;
            _userPreferMenuRepository = userPreferMenuRepository;
            _agencyUserRepository = agencyUserRepository;
            _parkAreaRepository = parkAreaRepository;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 新增系统用户
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task.</returns>
        public async Task<Result> AddSysUserAsync(CreateUserInput input)
        {
            var user = input.MapTo<User>();
            user.Password = new PasswordHasher().HashPassword(input.Password);
            user.IsActive = true;
            user.UserType = UserType.SysUser;

            //(await UserManager.CreateAsync(user, input.Password)).CheckErrors();

            var result = await UserManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                var sysUser = new SysUser() { User = user };
                await _sysUserRepository.InsertAndGetIdAsync(sysUser);

                return Result.Ok();
            }
            else
            {
                return Result.FromError(result.Errors.JoinAsString(", "));
            }
        }

        /// <summary>
        /// 添加用户喜好菜单列表
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddUserPreferMenuAsnyc(string menuName, string permissionName)
        {
            var entity = new UserPreferMenu
            {
                MenuName = menuName,
                PermissionName = permissionName
            };
            await _userPreferMenuRepository.InsertAndGetIdAsync(entity);

            return Result.Ok();
        }

        /// <summary>
        /// 删除用户喜好菜单列表
        /// </summary>
        /// <returns></returns>
        public async Task<Result> DeleteUserPreferMenuAsnyc(string menuName, string permissionName)
        {
            await _userPreferMenuRepository.DeleteAsync(m => m.MenuName == menuName && m.PermissionName == permissionName);

            return Result.Ok();
        }

        /// <summary>
        /// 获取代理商用户信息
        /// </summary>
        public Task<TDto> GetAgencyUserAsync<TDto>(IQuery<AgencyUser> query)
        {
            return _agencyUserRepository.GetAll().FirstOrDefaultAsync<AgencyUser, TDto>(query);
        }

        /// <summary>
        /// 根据Id获取系统用户
        /// </summary>
        /// <typeparam name="TDto">The type of the return.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TDto&gt;.</returns>
        public Task<TDto> GetSysUserByIdAsync<TDto>(long id)
        {
            return _sysUserRepository.GetAll().Where(o => o.Id == id).ProjectTo<TDto>()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 删除系统用户
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> DeleteSysUserAsync(long id)
        {
            var sysUser = await _sysUserRepository.GetAll().FirstOrDefaultAsync(o => o.Id == id);
            if (sysUser == null)
            {
                return Result.FromCode(ResultCode.NoRecord);
            }

            var result = await _userManager.DeleteAsync(sysUser.User, UserType.SysUser);
            if (!result.Succeeded)
            {
                return Result.FromError(result.Errors.JoinAsString(","));
            }

            await _sysUserRepository.DeleteAsync(sysUser);

            return Result.Ok();
        }

        /// <summary>
        /// 删除代理商用户
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> DeleteAgencyUserAsync(long id)
        {
            var agencyUser = await _agencyUserRepository.FirstOrDefaultAsync(id);
            if (agencyUser == null)
                return Result.FromCode(ResultCode.NoRecord);

            var result = await _userManager.DeleteAsync(agencyUser.User, UserType.AgencyUser);
            if (!result.Succeeded)
            {
                return Result.FromError(result.Errors.JoinAsString(","));
            }

            await _agencyUserRepository.DeleteAsync(agencyUser);
            return Result.Ok();
        }

        /// <summary>
        /// 系统用户分页查询方法
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        public Task<PageResult<TDto>> GetSysUsersByPageAsync<TDto>(IPageQuery<SysUser> pageQuery)
        {
            var query = _sysUserRepository.GetAll();

            var data = query.ToPageResultAsync<SysUser, TDto>(pageQuery);

            return data;
        }

        /// <summary>
        /// 系统用户分页查询方法，根据数据权限过滤，只显示当前用户同级及下级或者未设置数据权限的用户
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        public async Task<PageResult<TDto>> GetDataPermissionSysUsersByPageAsync<TDto>(IPageQuery<SysUser> pageQuery)
        {
            var userId = AbpSession.GetUserId();
            var parkAreaId = await _sysUserRepository.GetAll().Where(o => o.Id == userId)
                .Select(o => o.ParkAreaId).FirstOrDefaultAsync();

            var query = _sysUserRepository.GetAll();

            if (parkAreaId.HasValue)
            {
                var parkAreas = await _parkAreaRepository.GetParkAreaChildrenQuery(parkAreaId.Value)
                    .Select(o => o.Id).ToListAsync();

                parkAreas.Add(parkAreaId.Value);

                query = query.Where(o => !o.ParkAreaId.HasValue || parkAreas.Contains(o.ParkAreaId.Value));
            }

            var data = await query.ToPageResultAsync<SysUser, TDto>(pageQuery);
            return data;
        }

        /// <summary>
        /// 所有用户分页查询方法
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        public Task<PageResult<TDto>> GetAllUsersByPageAsync<TDto>(IPageQuery<User> pageQuery)
        {
            var query = _userRepository.AsNoTracking();

            var data = query.ToPageResultAsync<User, TDto>(pageQuery);

            return data;
        }

        /// <summary>
        /// get user name by identifier as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;UserDto&gt;.</returns>
        [DisableAuditing]
        public Task<string> GetUserNameByIdAsync(long id)
        {
            return _cacheManager.GetUserNameCache().GetAsync(id.ToString(),
                () => _userStore.Users.Where(o => o.Id == id).Select(o => o.Name).FirstOrDefaultAsync());
        }

        /// <summary>
        /// 根据条件获取用户喜好菜单列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetUserPreferMenusListAsnyc<TDto>(IQuery<UserPreferMenu> query)
        {
            return await _userPreferMenuRepository.AsNoTracking().ToListAsync<UserPreferMenu, TDto>(query);
        }

        //Example for primitive method parameters.
        /// <summary>
        /// Removes from role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Task.</returns>
        public async Task RemoveFromRole(long userId, string roleName)
        {
            (await UserManager.RemoveFromRoleAsync(userId, roleName)).CheckErrors();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdatePasswordAsync(UpdatePasswordInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            if (user == null)
            {
                return Result.FromCode(ResultCode.NoRecord);
            }

            if (!await _userManager.CheckPasswordAsync(user, input.OldPassword))
            {
                return Result.FromCode(ResultCode.SpaFailed);
            }

            if (input.Password.Equals(input.OldPassword))
            {
                return Result.FromCode(ResultCode.WrongNewPassword, "新密码不能和旧密码相同");
            }

            var result = await _userManager.ChangePasswordAsync(user, input.Password);
            if (result.Succeeded)
            {
                return Result.Ok();
            }

            return Result.FromCode(ResultCode.Fail);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> ResetPasswordAsync(ResetPasswordInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            if (user == null)
            {
                return Result.FromCode(ResultCode.NoRecord);
            }

            var result = await _userManager.ChangePasswordAsync(user, input.Password);
            if (result.Succeeded)
            {
                return Result.Ok();
            }

            return Result.FromCode(ResultCode.Fail);
        }

        /// <summary>
        /// 修改手机号
        /// </summary>
        public async Task<Result> UpdatePhoneByOldPhoneAsync(long userId, UpdatePhoneInput input)
        {
            if (!Regex.IsMatch(input.NewPhoneNum.Trim(), @"^(13|15|17|18)[0-9]{9}$"))
            {
                return Result.FromError("请输入正确的手机号格式！");
            }

            var user = await _userRepository.FirstOrDefaultAsync(o => o.Id == userId);
            if (user == null)
            {
                return Result.FromCode(ResultCode.NoRecord);
            }

            user.PhoneNumber = input.NewPhoneNum;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result.FromError(result.Errors.JoinAsString(","));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdateUserAsync(ApplicationDto.Users.UpdateUserInput input)
        {
            var user = await _userRepository.GetAsync(input.Id);
            input.MapTo(user);

            await _userManager.UpdateAsync(user);

            return Result.Ok();
        }

        /// <summary>
        /// 修改用户数据权限
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdateUserDataPermissionAsync(UpdateUserDataPermissionInput input)
        {
            var user = await _sysUserRepository.GetAsync(input.Id);

            //update roles
            await _userManager.SetDataPermission(user, input.ParkAreaId);

            return Result.Ok();
        }

        /// <summary>
        /// 修改用户角色
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdateUserRolesAsync(UpdateUserRoleInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);

            var checkedRoles = input.Roles.Distinct().ToArray();

            //update roles
            await _userManager.SetRoles(user, checkedRoles);

            return Result.Ok();
        }

        /// <summary>
        /// 接收客户端用户修改密码（数据同步）
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdatePasswordDatasync(ChangePasswordDto input)
        {
            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user == null)
                return Result.FromCode(ResultCode.NoRecord);
            else
            {
                var result = await _userManager.ChangePasswordAsync(user, input.NewPassword);
                if (result.Succeeded)
                {
                    return Result.Ok();
                }

                return Result.FromCode(ResultCode.Fail);
            }
        }
        #endregion Methods
    }
}