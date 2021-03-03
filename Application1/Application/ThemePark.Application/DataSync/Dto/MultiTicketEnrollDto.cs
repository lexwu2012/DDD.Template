using System;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 套票登记Dto
    /// </summary>
    public class MultiTicketEnrollDto
    {
        /// <summary>
        /// 来源公园
        /// </summary>
        public int FromParkid { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 指纹数据
        /// </summary>
        public byte[] Finger { get; set; }


        /// <summary>
        /// 照片数据
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// 登记通道号
        /// </summary>
        public int TerminalId { get; set; }

        /// <summary>
        /// 入园时间
        /// </summary>
        public DateTime EnrollTime { get; set; }
    }
}