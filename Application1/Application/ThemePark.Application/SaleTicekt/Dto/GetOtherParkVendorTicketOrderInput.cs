using System.Collections.Generic;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 获取其他公园预订票
    /// </summary>
    public class SearchOtherParkVendorOrderInput
    {
        /// <summary>
        /// 身份证或取票码
        /// </summary>
        public string PidOrTicketCode { get; set; }

        /// <summary>
        /// 其他公园ID
        /// </summary>
        public List<int> ParkIds { get; set; }
    }
}
