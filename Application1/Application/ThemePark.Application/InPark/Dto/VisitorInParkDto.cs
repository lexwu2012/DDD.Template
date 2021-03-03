using Abp.AutoMapper;
using System;
using ThemePark.Core.InPark;

namespace ThemePark.Application.InPark.Dto
{
    /// <summary>
    /// 入园单
    /// </summary>
    [AutoMap(typeof(VisitorInPark))]
    public class VisitorInParkDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 入园公园编号
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 入园公园
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 条码号
        /// </summary>    
        public string Barcode { get; set; }

        /// <summary>
        /// 入园通道
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 入园的人数
        /// </summary>    
        public int Persons { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
