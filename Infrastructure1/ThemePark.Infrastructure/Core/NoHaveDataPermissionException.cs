using Abp;

namespace ThemePark.Infrastructure.Core
{
    /// <summary>
    /// 没有数据权限
    /// </summary>
    /// <seealso cref="Abp.AbpException" />
    public class HaveNoDataPermissionException : AbpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HaveNoDataPermissionException" /> class.
        /// </summary>
        public HaveNoDataPermissionException() : base("没有操作数据的权限")
        {
            
        }
    }
}
