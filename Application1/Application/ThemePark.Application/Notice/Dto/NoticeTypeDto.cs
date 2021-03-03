using Abp.AutoMapper;
using Abp.Application.Services.Dto;

namespace ThemePark.Application.Notice.Dto
{
    /// <summary>
    /// 公告类型
    /// </summary>
    [AutoMap(typeof(ThemePark.Core.Notice.NoticeType))]
    public class NoticeTypeDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
