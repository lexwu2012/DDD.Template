using Abp.AutoMapper;
using ThemePark.Core.OrderTrack;

namespace ThemePark.Application.OrderTrack.Dto
{
    [AutoMapTo(typeof(VendorTicketTrack))]
    public class TicketTrackInput
    {
        /// <summary>
        /// 终端号
        /// </summary>
        public int TerminalId { get; set; }

        /// <summary>
        /// 查询订单使用的关键字
        /// </summary>
        public string SearchCode { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string TOHeaderId { get; set; }


        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeinfoId { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        public int ParkId { get; set; }


    }
}
