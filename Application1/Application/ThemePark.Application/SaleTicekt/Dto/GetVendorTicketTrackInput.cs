using System;
using Abp.AutoMapper;
using ThemePark.Core.OrderTrack;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 获取订单记录
    /// </summary>
    [AutoMapFrom(typeof(VendorTicketTrack))]
    public class GetVendorTicketTrackInput
    {
        /// <summary>
        /// 机器配置ID
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// TOHeaderId 或者 TradeinfoId
        /// </summary>
        public string SearchId { get; set; }



    }
}
