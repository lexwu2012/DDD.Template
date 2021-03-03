using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThemePark.Common;
using ThemePark.Core.InPark;

namespace ThemePark.Application.InPark.Dto
{
    /// <summary>
    /// 入园单
    /// </summary>
    [AutoMap(typeof(InParkBill))]
    public class InParkBillDto
    {

        /// <summary>
        /// 入园单条码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 入园单编码
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>    
        [Required]
        public int ParkId { get; set; }

        /// <summary>
        /// 单位
        /// </summary>    
        [StringLength(50)]
        public string Company { get; set; }

        /// <summary>
        /// 工作事由
        /// </summary>    
        [StringLength(600)]
        public string Reasons { get; set; }

        /// <summary>
        /// 人数
        /// </summary>    
        [Required]
        public int PersonNum { get; set; }

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
        /// 确认人
        /// </summary>    
        [StringLength(20)]
        public string ApprovedBy { get; set; }

        /// <summary>
        /// 入园类型
        /// </summary>
        public InParkType InParkType { get; set; }

        /// <summary>
        /// 入园类型名称
        /// </summary>
        public string InParkTypeName => InParkType.DisplayName();

        /// <summary>
        /// 工作类型
        /// </summary>    
        public WorkType? WorkType { get; set; }

        /// <summary>
        /// 工作类型名称
        /// </summary>
        public string WorkTypeName => WorkType == null ? "" : WorkType.DisplayName();

        /// <summary>
        /// 入园通道
        /// </summary>
        public InParkChannelType InParkChannel { get; set; }

        /// <summary>
        /// 入园通道名称
        /// </summary>
        public string InParkChannelName => InParkChannel.DisplayName();


        /// <summary>
        /// 入园时段
        /// </summary>
        public InParkTimeType InParkTime { get; set; }

        /// <summary>
        /// 入园时段名称
        /// </summary>
        public string InParkTimeName => InParkTime.DisplayName();


        /// <summary>
        /// 有效开始日期
        /// </summary>    
        public System.DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>    
        public string ValidStartDate2 => ValidStartDate.ToString("yyyy-MM-dd");

        /// <summary>
        /// 有效天数
        /// </summary>    
        public int ValidDays { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(300)]
        public string Remark { get; set; }

        /// <summary>
        /// 入园须知
        /// </summary>
        public string InparkNotice { get; set; }

        /// <summary>
        /// 入园单状态
        /// </summary>
        public InParkBillState InParkBillState { get; set; }

        /// <summary>
        /// 入园状态说明
        /// </summary>
        public string StateName => InParkBillState.DisplayName();

        /// <summary>
        /// 操作人
        /// </summary>
        public UserDto User { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<VisitorInParkDto> BillVisitorInParks {get;set;}

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        public string LastModifierUserName { get; set; }

    }
}
