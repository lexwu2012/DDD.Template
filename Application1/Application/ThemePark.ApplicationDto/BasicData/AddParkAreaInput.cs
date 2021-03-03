using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// 新增公园区域
    /// </summary>
    [AutoMapTo(typeof(ParkArea))]
    public class AddParkAreaInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 只有叶子节点值不为空
        /// </summary>
        public int? ParkId { get; set; }
    }
}
