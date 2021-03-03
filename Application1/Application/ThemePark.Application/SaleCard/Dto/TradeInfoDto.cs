using Abp.AutoMapper;
using ThemePark.Common;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 支付信息
    /// </summary>
    [AutoMap(typeof(TradeInfo))]
    public class TradeInfoDto
    {
        /// <summary>
        /// 支付类型编号
        /// </summary>

        public PayType PayModeId { get; set; }

        /// <summary>
        /// 支付类型名称
        /// </summary>
        public string PayModeName => PayModeId.DisplayName();

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}
