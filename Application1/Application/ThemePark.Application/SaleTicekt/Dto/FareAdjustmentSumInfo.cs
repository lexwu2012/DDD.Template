using Abp.AutoMapper;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 补票销售数据
    /// </summary>
    [AutoMap(typeof(ExcessFare))]
    public class FareAdjustmentSumInfo
    {
        /// <summary>
        /// 公园编号
        /// </summary>    
        public int ParkId { get; set; }

        /// <summary>
        /// 面额
        /// </summary>    
        public decimal Denomination { get; set; }

        /// <summary>
        /// 张数
        /// </summary>    
        public int Qty { get; set; }
        /// <summary>
        /// 补票金额
        /// </summary>    
        public decimal Amount { get; set; }
    }
}
