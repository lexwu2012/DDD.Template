using System.Threading.Tasks;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// Interface IUserInfoService
    /// </summary>
    public interface IUserInfoService
    {
        /// <summary>
        /// 返回指定用户的用户名
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>System.String.</returns>
        Task<string> GetUserNameByIdAsync(long userId);
    }
}
