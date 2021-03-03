using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.ApplicationDto.Agencies
{
    /// <summary>
    /// 可带队类型Dto
    /// </summary>
    [AutoMap(typeof(GroupType))]
    public class GroupTypeDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 带队类型名称
        /// </summary>    
        public string TypeName { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 默认排序
        /// </summary>
        public int SerialNumber { get; set; }
    }
}
