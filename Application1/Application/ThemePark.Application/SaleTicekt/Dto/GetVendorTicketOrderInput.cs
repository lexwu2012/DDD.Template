namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 获取自助售票机预订票
    /// </summary>
    public class SearchVendorTicketOrderInput
    {
        /// <summary>
        /// 身份证或取票码
        /// </summary>
        public string PidOrTicketCode { get; set; }

    }
}
