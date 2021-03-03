namespace ThemePark.Infrastructure.Core
{
    /// <summary>
    /// 序号
    /// </summary>
    public interface ISerialable
    {
        /// <summary>
        /// 默认序号
        /// </summary>
        int SerialNumber { get; set; }
    }
}
