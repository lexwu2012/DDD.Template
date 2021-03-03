using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 套票作废Dto
    /// </summary>
    public class MultiTicketCancelDto
    {
        /// <summary>
        /// 票类型 团体、散客
        /// </summary>
        public TicketCategory TicketCategory { get; set; }

        ///// <summary>
        ///// 来源公园
        ///// </summary>
        //public int FromParkid { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }

        ///// <summary>
        ///// 票数量
        ///// </summary>
        //public int Qty { get; set; }
    }
}