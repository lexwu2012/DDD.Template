using ThemePark.Application.Trade.Dto;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 账户操作Dto
    /// </summary>
    public class AccountDto
    {
        /// <summary>
        /// 客户端传入的支付信息
        /// </summary>
        public TradeInfoInput TradeInfos { get; set; }

        /// <summary>
        /// 代理商Id
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OpType OpType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
