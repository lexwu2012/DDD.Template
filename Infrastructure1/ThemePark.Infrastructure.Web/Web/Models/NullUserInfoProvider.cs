using System;
using Abp.Dependency;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Infrastructure.Web.Models
{
    /// <summary>
    /// Class NullUserInfoProvider.
    /// </summary>
    /// <seealso cref="IUserInfoProvider" />
    public class NullUserInfoProvider : IUserInfoProvider, ISingletonDependency
    {
        /// <summary>
        /// Gets the user information service.
        /// </summary>
        /// <returns>IUserInfoService.</returns>
        public IUserInfoService GetUserInfoService()
        {
            throw new NotImplementedException();
        }
    }
}
