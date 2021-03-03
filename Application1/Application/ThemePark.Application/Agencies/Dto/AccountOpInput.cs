using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Web.Models;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 帐户操作表
    /// </summary>
    [AutoMap(typeof(AccountOp))]
    public class AccountOpInput: CreationModel
    {
        /// <summary>
        /// 操作类型 充值 消费
        /// </summary>    
        [Required]
        public OpType OpType { get; set; }
        /// <summary>
        /// 帐户编号
        /// </summary>    
        public int AccountId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>    
        public decimal? Cash { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(100)]
        public string Remark { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }
    }
}
