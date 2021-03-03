namespace ThemePark.Infrastructure.Security.DataProtection
{
    /// <summary>
    /// 限定时间的数据保护器工厂
    /// </summary>
    public interface ITimeLimitedDataProtectionProvider
    {
        /// <summary>
        /// 创建一个数据保护器
        /// </summary>
        /// <param name="purposes">保护数据增加的额外的标识</param>
        ITimeLimitedDataProtector Create(params string[] purposes);
    }
}