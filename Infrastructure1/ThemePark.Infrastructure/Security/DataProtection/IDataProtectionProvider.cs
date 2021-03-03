
namespace ThemePark.Infrastructure.Security.DataProtection
{
    /// <summary>
    /// 数据保护器工厂
    /// </summary>
    public interface IDataProtectionProvider
    {
        /// <summary>
        /// 创建一个数据保护器
        /// </summary>
        /// <param name="purposes">保护数据增加的额外的标识</param>
        IDataProtector Create(params string[] purposes);
    }
}
