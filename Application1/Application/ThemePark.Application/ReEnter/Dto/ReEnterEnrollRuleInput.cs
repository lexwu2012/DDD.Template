using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.ReEnter.Dto
{
    /// <summary>
    /// 二次入园规则输入参数
    /// </summary>
    public class ReEnterEnrollRuleInput
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 二次入园规则名称
        /// </summary>    
        [Required]
        [StringLength(20)]
        public string RuleName { get; set; }

        /// <summary>
        /// 有效次数
        /// </summary>    
        public int ValidCount { get; set; }

        /// <summary>
        /// 开始登记时间
        /// </summary>    
        public System.DateTime EnrollStartTime { get; set; }

        /// <summary>
        /// 结束登记时间
        /// </summary>    
        public System.DateTime EnrollEndTime { get; set; }

        /// <summary>
        /// 限定时间（分钟）
        /// </summary>    
        public int LimitedTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 验证指纹
        /// </summary>
        /// <value><c>true</c> if need check finger; otherwise, <c>false</c>.</value>
        public bool NeedCheckFp { get; set; }

        /// <summary>
        /// 验证照片
        /// </summary>
        /// <value><c>true</c> if need check photo; otherwise, <c>false</c>.</value>
        public bool NeedCheckPhoto { get; set; }
    }
}
