using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 代理商账户dto
    /// </summary>
    [AutoMap(typeof(Agency))]
    public class AgencyAccountDto
    {
        /// <summary>
        /// 代理商账户Id
        /// </summary>
        public int AccountId { get; set; }
    }
}
