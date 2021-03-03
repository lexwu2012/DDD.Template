using Abp.AutoMapper;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using ThemePark.Application.Users.Dto;
using ThemePark.Application.Users.Interfaces;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Core.Agencies.Repositories;
using ThemePark.Core.Authorization.Users;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using Microsoft.AspNet.Identity;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Core.Users;
using ThemePark.Infrastructure;
using System;

namespace ThemePark.Application.Users
{
    /// <summary>
    /// 代理商用户
    /// </summary>
    /// <seealso cref="ThemePark.Application.ThemeParkAppServiceBase" />
    /// <seealso cref="ThemePark.Application.Users.Interfaces.IAgencyUserAppService" />
    public class AgencyUserAppService : ThemeParkAppServiceBase, IAgencyUserAppService
    {
        private readonly IAgencyUserRepository _agencyUserRepository;

        private readonly IAgencyUserDomainService _agencyUserDomainService;

        private readonly UserManager _userManager;

        private readonly IParkAgencyRepository _parkAgencyRepository;

        /// <summary>
        /// The _sysuser repository
        /// </summary>
        private readonly IRepository<SysUser, long> _sysUserRepository;

        private readonly IParkAreaRepository _parkAreaRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyUserAppService" /> class.
        /// </summary>
        /// <param name="agencyUserRepository">The agency user repository.</param>
        /// <param name="userManager">The user manager.</param>
        public AgencyUserAppService(IAgencyUserRepository agencyUserRepository, UserManager userManager, IAgencyUserDomainService agencyUserDomainService, IRepository<SysUser, long> sysUserRepository, IParkAreaRepository parkAreaRepository, IParkAgencyRepository parkAgencyRepository)
        {
            _agencyUserRepository = agencyUserRepository;
            _userManager = userManager;
            _agencyUserDomainService = agencyUserDomainService;
            _sysUserRepository = sysUserRepository;
            _parkAreaRepository = parkAreaRepository;
            _parkAgencyRepository = parkAgencyRepository;
        }

        /// <summary>
        /// 根据旅行社Id获取系统用户
        /// </summary>
        public async Task<TDto> GetAgencyUserByIdAsync<TDto>(int agencyId)
        {
            var user = await _agencyUserRepository.FirstOrDefaultAsync(o => o.AgencyId == agencyId);
            return user.MapTo<TDto>();
        }

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        public Task<PageResult<TDto>> GetAgencyUsersByPageAsync<TDto>(IPageQuery<AgencyUser> pageQuery)
        {
            return _agencyUserRepository.AsNoTracking().ToPageResultAsync<AgencyUser, TDto>(pageQuery); ;
        }

        /// <summary>
        /// 旅行社用户分页查询方法，根据数据权限过滤，只显示当前用户数据权限下的旅行社用户
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        public async Task<PageResult<TDto>> GetDataPermissionAgencyUsersByPageAsync<TDto>(IPageQuery<AgencyUser> pageQuery)
        {
            var userId = AbpSession.GetUserId();
            var parkAreaId = await _sysUserRepository.GetAll().Where(o => o.Id == userId)
                .Select(o => o.ParkAreaId).FirstOrDefaultAsync();

            var query = _agencyUserRepository.GetAll();

            if (parkAreaId.HasValue)
            {
                var parks = await _parkAreaRepository.GetParkNodesQuery(parkAreaId.Value)
                    .Select(o => o.ParkId.Value).ToListAsync();

                var agencies = await _parkAgencyRepository.GetAll().Where(o => parks.Contains(o.ParkId))
                    .Select(o => o.AgencyId).ToListAsync();
                agencies = agencies.Distinct().ToList();

                query = query.Where(o => agencies.Contains(o.AgencyId));
            }
            else
            {
                return new PageResult<TDto>();
            }

            var data = await query.ToPageResultAsync<AgencyUser, TDto>(pageQuery);
            return data;
        }

        ///// <summary>
        ///// 新增代理商用户
        ///// </summary>
        ///// <param name="input">The input.</param>
        ///// <returns>Task&lt;Result&gt;.</returns>
        //public async Task<Result> AddAgencyUserAsync(AddAgencyUserInput input)
        //{
        //    var user = input.User.MapTo<User>();
        //    user.Password = new PasswordHasher().HashPassword(input.User.Password);
        //    //user.IsActive = true;

        //    var identityResult = await _userManager.CreateAsync(user);
        //    if (identityResult.Succeeded)
        //    {
        //        await _agencyUserDomainService.CreateAgencyUserAsync(user, input.AgencyId);
        //        return Result.Ok();
        //    }
        //    else
        //    {
        //        return Result.FromError(identityResult.Errors.JoinAsString(", "));
        //    }
        //}

        /// <summary>
        /// 修改密码
        /// </summary>
        public async Task<Result> UpdatePasswordByOldPswAsync(UpdateAgencyPswInput input)
        {
            var user = await _agencyUserRepository.FirstOrDefaultAsync(o => /*o.AgencyId == input.AgencyId &&*/ o.User.Password == input.OldPassword);
            if (user != null)
            {
                user.User.Password = input.Password;
                await _agencyUserRepository.UpdateAsync(user);
                return Result.Ok();
            }
            return Result.FromCode(ResultCode.InvalidData, "密码错误！");

        }


        /// <summary>
        /// 重置密码
        /// </summary>
        public async Task<Result> ResetPasswordAsync(int agencyId, string psd)
        {
            var user = await _agencyUserRepository.FirstOrDefaultAsync(o => o.AgencyId == agencyId);
            if (user != null)
            {
                user.User.Password = psd;
                await _agencyUserRepository.UpdateAsync(user);
                return Result.Ok();
            }
            return Result.FromCode(ResultCode.NoRecord);
        }


        /// <summary>
        /// 根据条件查询代理商用户
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetAgencyUserAsync<TDto>(IQuery<AgencyUser> query)
        {
            return await _agencyUserRepository.GetAll().FirstOrDefaultAsync<AgencyUser, TDto>(query);
        }

        /// <summary>
        /// 根据Id获取旅行社用户
        /// </summary>
        public async Task<TDto> GetAgencyUserByIdAsync<TDto>(long id)
        {
            return await _agencyUserRepository.AsNoTracking().FirstOrDefaultAsync<AgencyUser, TDto>(new Query<AgencyUser>(o => o.Id == id));

        }

        /// <summary>
        /// 绑定微信账号
        /// </summary>
        /// <param name="weChatNo">微信账号</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> BindWeChat(string weChatNo)
        {
            if (await _agencyUserRepository.GetAll().AnyAsync(o => o.WeChatNo == weChatNo))
            {
                return Result.FromCode(ResultCode.Fail, "微信已经绑定了其他账号");
            }

            var user = await _agencyUserRepository.GetAsync(AbpSession.GetUserId());
            if (await _agencyUserDomainService.BindWeChatNo(user, weChatNo) == null)
            {
                return Result.FromCode(ResultCode.Fail, "已经绑定过微信号");
            }

            return Result.Ok();
        }

        /// <summary>
        /// 解绑微信账号
        /// </summary>
        /// <param name="weChatNo">微信账号</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UnbindWeChat(string weChatNo)
        {
            var user = await _agencyUserRepository.GetAsync(AbpSession.GetUserId());
            if (await _agencyUserDomainService.UnbindWeChatNo(user, weChatNo) == null)
            {
                return Result.FromCode(ResultCode.Fail, "解绑失败，绑定的账号不匹配");
            }

            return Result.Ok();
        }

        /// <summary>
        /// 微信绑定状态
        /// </summary>
        /// <param name="agencyUserId">代理商账号</param>
        /// <param name="weChatNo">微信账号</param>
        /// <returns>Task&lt;ThirdPartyState&gt;.</returns>
        public async Task<ThirdPartyState> WeChatBoundState(long agencyUserId, string weChatNo)
        {
            var userId = await _agencyUserRepository.GetAll().Where(o => o.WeChatNo == weChatNo).Select(o => o.Id)
                .FirstOrDefaultAsync();

            //微信没有被使用
            if (userId == 0)
            {
                //代理商用户没有绑定微信
                if (await _agencyUserRepository.GetAll().CountAsync(o => o.Id == agencyUserId
                    && string.IsNullOrEmpty(o.WeChatNo)) == 1)
                {
                    return ThirdPartyState.Available;
                }

                return ThirdPartyState.Disable;
            }
            else
            {
                return userId == AbpSession.GetUserId() ? ThirdPartyState.Bound : ThirdPartyState.BoundOther;
            }
        }

        /// <summary>
        /// 验证代理商账户密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="hostIp"></param>
        /// <returns></returns>
        public Task<Result<User>> CheckAgencyApiUserAsync(string userName, string password, string hostIp)
        {
            return _agencyUserDomainService.CheckAgencyApiUserAsync(userName, password, hostIp);
        }
    }
}
