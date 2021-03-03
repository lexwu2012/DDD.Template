using System;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 套票入园
    /// </summary>
    public class MultiTicketInparkDto
    {
        /// <summary>
        /// 票类型 团体、散客
        /// </summary>
        public TicketCategory TicketClassType { get; set; }

        /// <summary>
        /// 来源公园
        /// </summary>
        public int FromParkid { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }


        /// <summary>
        /// 备注
        /// </summary>

        public string Remark { get; set; }

        /// <summary>
        /// 入园人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 入园通道号
        /// </summary>
        public int TerminalId { get; set; }

        /// <summary>
        /// 入园时间
        /// </summary>
        public DateTime InparkTime { get; set; }

        /// <summary>
        /// 套票状态
        /// </summary>
        public TicketSaleStatus TicketSaleStatus { get; set; }
    }
}