using Abp.AutoMapper;
using System;
using ThemePark.Core.InPark;

namespace ThemePark.Application.InPark.Dto
{
    /// <summary>
    /// 入园dto
    /// </summary>
    [AutoMap(typeof(TicketInPark))]
    public class TicketInParkDto
    {
        /// <summary>
        /// 入园公园
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 基础票类Id
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 入园时间
        /// </summary>    
        public DateTime InParkTime { get; set; }

        /// <summary>
        /// 入园次数
        /// </summary>    
        public int Qty { get; set; }

        /// <summary>
        /// 入园通道
        /// </summary>    
        public int TerminalId { get; set; }
    }
}
