using Abp;

namespace ThemePark.Infrastructure
{
    public static class ThemeParkSessionExtenstions
    {
        /// <summary>
        /// 获取当前用户Id，如果用户未登录，则抛出 <see cref="AbpException"/>
        /// </summary>
        /// <exception cref="AbpException">如果用户未登录，则抛出 <see cref="AbpException"/></exception>
        public static long GetUserId(this IThemeParkSession session)
        {
            if(!session.UserId.HasValue)
                throw new AbpException("Session.UserId is null! Probably, user is not logged in.");

            return session.UserId.Value;
        }

        /// <summary>
        /// 获取当前用户关联代理商Id
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="AbpException">如果用户未登录</exception>
        public static int GetAgencyId(this IThemeParkSession session)
        {
            if (!session.AgencyId.HasValue)
                throw new AbpException("Session.UserId is null! Probably, user is not logged in.");

            return session.AgencyId.Value;
        }
    }
}