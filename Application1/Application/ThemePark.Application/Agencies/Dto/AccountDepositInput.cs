using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicData;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(AccountOp))]
    public class AccountDepositInput
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
        /// 方特币
        /// </summary>    
        public decimal? Ftb { get; set; }

        /// <summary>
        /// 票券
        /// </summary>    
        public decimal? Coupon { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        [Required]
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(100)]
        public string Remark { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long CreatorUserId { get; set; }

    }
}
