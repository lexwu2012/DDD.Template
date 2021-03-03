namespace ThemePark.Infrastructure.EntityFramework
{
    /// <summary>
    /// 动态条件 常量定义
    /// </summary>
    public sealed class DataFilters
    {
        /// <summary>
        /// The park permission
        /// </summary>
        public const string ParkPermission = "ParkPermission";

        /// <summary>
        /// The agency permission
        /// </summary>
        public const string AgencyPermission = "AgencyPermission";

        /// <summary>
        /// 动态条件参数定义
        /// </summary>
        public sealed class Parameters
        {
            /// <summary>
            /// The park permission key
            /// </summary>
            public const string ParkPermissionKey = "parkIds";
        }
    }
}
