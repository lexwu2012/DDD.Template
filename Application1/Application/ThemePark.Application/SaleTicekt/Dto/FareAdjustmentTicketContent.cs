namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 补票打印内容
    /// </summary>
    public class FareAdjustmentTicketContent
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string Barcode = string.Empty;

        /// <summary>
        /// 面额
        /// </summary>    
        public string Denomination = string.Empty;

        /// <summary>
        /// 补票时间
        /// </summary>
        public string CreationTime = string.Empty;
    }
}
