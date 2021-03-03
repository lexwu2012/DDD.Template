using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 交易记录详情新增模型
    /// </summary>
    [AutoMapTo(typeof(TradeInfoDetail))]
    public class TradeInfoDetailInput
    {
        /// <summary>
        /// 支付类型编号
        /// </summary>    
        [Required]
        public PayType PayModeId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>    
        [Required(ErrorMessage ="支付金额不能小等于0"),Range(0,int.MaxValue)]
        public decimal Amount { get; set; }

    }
}
