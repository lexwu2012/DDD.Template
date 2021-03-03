using System;
using Abp.AutoMapper;
using ThemePark.Core.OrderTrack;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapFrom(typeof(VendorTicketTrack))]
    public class VendorTicketTrackDto
    {
        /// <summary>
        /// 机器配置ID
        /// </summary>
        public int TerminalId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeinfoId { get; set; }

        /// <summary>
        /// 追踪记录
        /// </summary>
        public string TrackLog { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
