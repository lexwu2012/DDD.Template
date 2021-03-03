using System;

namespace ThemePark.Infrastructure.Security.DataProtection.Dpapi
{
    /// <summary>
    /// 使用 <see cref="System.Security.Cryptography.DpapiDataProtector"/> 加密数据
    /// </summary>
    public class DpapiDataProtectionProvider : IDataProtectionProvider
    {
        private readonly string _appName;

        /// <summary>
        /// 创建一个新的 <see cref="DpapiDataProtectionProvider"/>
        /// </summary>
        public DpapiDataProtectionProvider(string appName)
        {
            _appName = appName;
        }

        /// <summary>
        /// 创建一个数据保护器
        /// </summary>
        /// <param name="purposes">保护数据增加的额外的标识</param>
        public IDataProtector Create(params string[] purposes)
        {
            if (purposes == null)
                throw new ArgumentNullException(nameof(purposes));
            return new DpapiDataProtector(_appName, purposes);
        }
    }
}
