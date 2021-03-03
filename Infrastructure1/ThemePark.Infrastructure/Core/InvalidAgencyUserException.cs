using Abp;

namespace ThemePark.Infrastructure.Core
{
    /// <summary>
    /// 无效的代理商用户
    /// </summary>
    /// <seealso cref="Abp.AbpException" />
    public class InvalidAgencyUserException : AbpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAgencyUserException"/> class.
        /// </summary>
        public InvalidAgencyUserException() : base("无效的代理商用户")
        {
            
        }
    }
}
