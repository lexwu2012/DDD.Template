using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ThemePark.Application.Notice.Dto
{
    /// <summary>
    /// 公告类型Input
    /// </summary>
    [AutoMap(typeof(ThemePark.Core.Notice.NoticeType))]
    public class NoticeTypeInput
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [Required(ErrorMessage ="公告类型名称不能为空")]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        [StringLength(50)]
        public string Remark { get; set; }
    }
}
