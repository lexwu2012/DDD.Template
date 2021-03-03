using ThemePark.Application.Trade.Dto;

namespace ThemePark.Application.SaleTicekt.Dto
{
    public class FareAdjustmentAddNewInput
    {
        /// <summary>
        /// 补票实体Dto
        /// </summary>
        public FareAdjustmentDto FareAdjustment { get; set; }

        /// <summary>
        /// 客户端传入的支付信息
        /// </summary>
        public TradeInfoInput PayInfo { get; set; }

        /// <summary>
        /// 界面传进来的补票张数
        /// </summary>
        public int Qty { get; set; }
    }
}
