using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.Trade.Dto
{
    /// <summary>
    /// 交易记录详情新增模型
    /// </summary>
    [AutoMap(typeof(TradeInfoDetail))]
    public class TradeInfoDetailDto : FullAuditedEntityDto<string>
    {
        /// <summary>
        /// 交易号
        /// </summary>    
        [Required]
        [StringLength(50)]
        public string Tradeno { get; set; }
        /// <summary>
        /// 支付类型编号
        /// </summary>    
        [Required]
        public PayType PayModeId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>    
        [Required]
        public decimal Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        [Required]
        public PayStatus PayStatus { get; set; }
    }
}
