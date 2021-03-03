using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Application.Agencies.Dto;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.AgentTicket.Dto
{
    [AutoMapFrom(typeof(ParkAgencyTypeGroupType))]
    public class GetParkAgencyTypeGroupTypeDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 代理商类型ID
        /// </summary>
        public int AgencyTypeId { get; set; }
        /// <summary>
        /// 团体类型ID
        /// </summary>
        public int GroupTypeId { get; set; }
        /// <summary>
        /// 代理商票类规则ID
        /// </summary>
        public int AgencyRuleId { get; set; }
        /// <summary>
        /// 团体类型
        /// </summary>
        public GroupTypeDto GroupType { get; set; }

        /// <summary>
        /// 公园
        /// </summary>
        public ParkDto Park { get; set; }

        /// <summary>
        /// 代理商类型
        /// </summary>
        public AgencyTypeDto AgencyType { get; set; }

        /// <summary>
        /// 代理商规则
        /// </summary>
        public AgencyRuleOutputDto AgencyRule { get; set; }


    }
}
