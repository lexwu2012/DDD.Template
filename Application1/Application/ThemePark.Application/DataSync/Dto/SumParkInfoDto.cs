using Abp.AutoMapper;
using System;
using ThemePark.Core.SumInfo;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 上传汇总信息Dto
    /// </summary>
    [AutoMapTo(typeof(SumParkInfo))]
    public class SumParkInfoDto
    {
        /// <summary>
        /// 公园编号
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal SaleAmount { get; set; }

        /// <summary>
        /// 入园人数
        /// </summary>
        public int InParkCount { get; set; }

        /// <summary>
        /// 汇总日期
        /// </summary>
        public DateTime SummarizeDate { get; set; }
    }
}
