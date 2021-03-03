using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(Agency))]
    public class AccountMoneyDto
    {
        /// <summary>
        /// 余额
        /// </summary>    
        [MapFrom(nameof(Agency.Account),nameof(Account.Balance))]
        public decimal Balance { get; set; }
    }
}
