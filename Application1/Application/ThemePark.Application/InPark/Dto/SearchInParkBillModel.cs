using System;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.InPark;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.InPark.Dto
{
    /// <summary>
    /// 查询入园单
    /// </summary>
    public class SearchInParkBillModel: PageSortInfo
    {
        /// <summary>
        /// 入园单条码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 入园日期
        /// </summary>
        public DateTime? EnterParkDate { get; set; }

        /// <summary>
        /// 入园类型
        /// </summary>
        public InParkType? InParkType { get; set; }

        /// <summary>
        /// 工作类型
        /// </summary>
        public WorkType? WorkType { get; set; }

        /// <summary>
        /// 入园通道
        /// </summary>
        public InParkChannelType? InParkChannel { get; set; }

        /// <summary>
        /// 入园时段
        /// </summary>
        public InParkTimeType? InParkTime { get; set; }

        /// <summary>
        /// 申请部门
        /// </summary>    
        [StringLength(50)]
        public string ApplyDept { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>    
        [StringLength(20)]
        public string ApplyBy { get; set; }

        /// <summary>
        /// 单位
        /// </summary>    
        [StringLength(50)]
        public string Company { get; set; }
    }
}
