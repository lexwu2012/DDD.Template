using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using ThemePark.Common;
using ThemePark.Core.Notice;

namespace ThemePark.Application.Notice.Dto
{
    /// <summary>
    /// 公告Dto
    /// </summary>
    [AutoMap(typeof(ThemePark.Core.Notice.Notice))]
    public class NoticeDto:FullAuditedEntityDto
    {
        /// <summary>
        /// 公园ID
        /// </summary>
        public string ParkIds { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 公告类型ID
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 可见系统
        /// </summary>
        public SystemType? SystemTypeId { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 排序序号
        /// </summary>
        public int SerialNumber { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? ReleaseTime { get; set; }
        /// <summary>
        /// 可见系统名称
        /// </summary>
        public string SystemTypeName => SystemTypeId?.DisplayName();
        
        /// <summary>
        /// 公告类型
        /// </summary>
        public NoticeTypeDto NoticeType { get; set; }
        


    }
    
}
