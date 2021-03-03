using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Notice;

namespace ThemePark.Application.Notice.Dto
{
    /// <summary>
    /// 添加公告Input
    /// </summary>
    [AutoMap(typeof(ThemePark.Core.Notice.Notice))]
    public class AddNoticeInput
    {
        /// <summary>
        /// 公告标题
        /// </summary>
        [Required]
        [StringLength(100)]
        [DisplayName("公告标题")]
        public string Title { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        [Required]
        [DisplayName("公告内容")]
        public virtual string Content { get; set; }
        
        /// <summary>
        /// 公告类型ID
        /// </summary>
        [Required]
        public int TypeId { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [StringLength(50)]
        [Required]
        [DisplayName("公告来源")]
        public string Source { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [StringLength(255)]
        [Required]
        [DisplayName("公告摘要")]
        public string Summary { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        [Required]
        [DisplayName("排序序号")]
        public int SerialNumber { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [DisplayName("发布时间")]
        public DateTime? ReleaseTime { get; set; }

        /// <summary>
        /// 公园
        /// </summary>
        public string ParkIds { get; set; }

        /// <summary>
        /// 公园
        /// </summary>
        [Required]
        [DisplayName("公园")]
        public List<int> ParkIdList { get; set; }

        /// <summary>
        /// 可显示系统
        /// </summary>
        public SystemType? SystemTypeId { get; set; }
        
    }
}
