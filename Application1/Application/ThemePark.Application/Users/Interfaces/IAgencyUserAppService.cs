using Abp.Application.Services;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Core.Authorization.Users;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Users.Interfaces
{
    /// <summary>
    /// 代理商用户
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService" />
    public interface IAgencyUserAppService : IApplicationService
    {
        /// <summary>
        /// 根据旅行社Id获取系统用户
        /// </summary>
        Task<TDto> GetAgencyUserByIdAsync<TDto>(int agencyId);

        /// <summary>
        /// 根据Id获取旅行社用户
        /// </summary>
        Task<TDto> GetAgencyUserByIdAsync<TDto>(long Id);

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetAgencyUsersByPageAsync<TDto>(IPageQuery<AgencyUser> pageQuery);

        /// <summary>
        /// 旅行社用户分页查询方法，根据数据权限过滤，只显示当前用户数据权限下的旅行社用户
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetDataPermissionAgencyUsersByPageAsync<TDto>(IPageQuery<AgencyUser> pageQuery);

        /// <summary>
        /// 新增代理商用户
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        //Task<Result> AddAgencyUserAsync(AddAgencyUserInput input);
        
        /// <summary>
        /// 重置密码
        /// </summary>
        Task<Result> ResetPasswordAsync(int agencyId,string psd);

        /// <summary>
        /// 根据条件查询代理商用户
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetAgencyUserAsync<TDto>(IQuery<AgencyUser> query);

        /// <summary>
        /// 绑定微信账号
        /// </summary>
        /// <param name="weChatNo">微信账号</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> BindWeChat(string weChatNo);

        /// <summary>
        /// 解绑微信账号
        /// </summary>
        /// <param name="weChatNo">微信账号</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UnbindWeChat(string weChatNo);

        /// <summary>
        /// 微信绑定状态
        /// </summary>
        /// <param name="agencyUserId">代理商账号</param>
        /// <param name="weChatNo">微信账号</param>
        /// <returns>Task&lt;ThirdPartyState&gt;.</returns>
        Task<ThirdPartyState> WeChatBoundState(long agencyUserId, string weChatNo);

        /// <summary>
        /// 验证登录信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="hostIp"></param>
        /// <returns></returns>
        Task<Result<User>> CheckAgencyApiUserAsync(string userName, string password, string hostIp);
    }
}
