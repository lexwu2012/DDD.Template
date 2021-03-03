using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 带队类型Input
    /// </summary>
    [AutoMap(typeof(GroupType))]
    public class GroupTypeInput
    {
        /// <summary>
        /// 带队类型名称
        /// </summary>
        [DisplayName("团体类型")]
        [Required(AllowEmptyStrings =false, ErrorMessage = "团体类型为必填项"),StringLength(20, ErrorMessage = "类型名称请少于20字")]
        public string TypeName { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [DisplayName("备注")]
        [StringLength(50, ErrorMessage = "请少于20字")]
        public string Remark { get; set; }

        /// <summary>
        /// 默认排序
        /// </summary>
        [Range(0, 255)]
        [DisplayName("默认排序")]
        public int SerialNumber { get; set; }
    }
}
