namespace ThemePark.Infrastructure.Core
{
    /// <summary>
    /// 数据权限接口
    /// </summary>
    public interface IParkPermission
    {
        /// <summary>
        /// 权限关联键
        /// </summary>
        int ParkId { get; set; }
    }
}
