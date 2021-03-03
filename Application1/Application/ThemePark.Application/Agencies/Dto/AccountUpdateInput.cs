using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 账户更新输入
    /// </summary>
    [AutoMap(typeof(Account))]
    public class AccountUpdateInput
    {
        /// <summary>
        /// 余额
        /// </summary>    
        public decimal Balance { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }        
    }
}
