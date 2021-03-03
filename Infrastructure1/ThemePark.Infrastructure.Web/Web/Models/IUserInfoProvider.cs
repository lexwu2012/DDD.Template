using ThemePark.Infrastructure.Application;

namespace ThemePark.Infrastructure.Web.Models
{
    /// <summary>
    /// Interface IUserInfoProvider
    /// </summary>
    public interface IUserInfoProvider
    {
        /// <summary>
        /// Gets the user information service.
        /// </summary>
        /// <returns>IUserInfoService.</returns>
        IUserInfoService GetUserInfoService();
    }
}
