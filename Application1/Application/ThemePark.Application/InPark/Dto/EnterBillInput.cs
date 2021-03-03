using System.ComponentModel.DataAnnotations;
using ThemePark.Core.InPark;

namespace ThemePark.Application.InPark.Dto
{
    /// <summary>
    /// 入园单打印输入
    /// </summary>
    public class EnterBillInput
    {
        /// <summary>
        /// 入园单编号
        /// </summary>
        [Required, StringLength(20)]
        public string BillNo { get; set; }

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
        /// 工作类型
        /// </summary>    
        public WorkType? WorkType { get; set; }

        /// <summary>
        /// 入园通道
        /// </summary>
        public InParkChannelType InParkChannel { get; set; }

        /// <summary>
        /// 入园时段
        /// </summary>
        public InParkTimeType InParkTime { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>    
        public System.DateTime ValidStartDate { get; set; }

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

    }
}
