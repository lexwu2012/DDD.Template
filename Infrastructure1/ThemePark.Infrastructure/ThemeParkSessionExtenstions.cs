using Abp;

namespace ThemePark.Infrastructure
{
    public static class ThemeParkSessionExtenstions
    {
        /// <summary>
        /// ��ȡ��ǰ�û�Id������û�δ��¼�����׳� <see cref="AbpException"/>
        /// </summary>
        /// <exception cref="AbpException">����û�δ��¼�����׳� <see cref="AbpException"/></exception>
        public static long GetUserId(this IThemeParkSession session)
        {
            if(!session.UserId.HasValue)
                throw new AbpException("Session.UserId is null! Probably, user is not logged in.");

            return session.UserId.Value;
        }

        /// <summary>
        /// ��ȡ��ǰ�û�����������Id
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="AbpException">����û�δ��¼</exception>
        public static int GetAgencyId(this IThemeParkSession session)
        {
            if (!session.AgencyId.HasValue)
                throw new AbpException("Session.UserId is null! Probably, user is not logged in.");

            return session.AgencyId.Value;
        }
    }
}