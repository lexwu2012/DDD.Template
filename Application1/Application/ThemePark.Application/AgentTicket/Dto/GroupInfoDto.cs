using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 团队信息Dto
    /// </summary>
    [AutoMap(typeof(GroupInfo))]
    public class GroupInfoDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupInfoDto()
        {
            GuideList = new List<GuideCustomerDto>();
            DriverList = new List<DriverCustomerDto>();
        }

        /// <summary>
        /// 导游列表
        /// </summary>
        public IList<GuideCustomerDto> GuideList { get; set; }

        /// <summary>
        /// 司陪列表
        /// </summary>
        public IList<DriverCustomerDto> DriverList { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 带队类型编号
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 旅游团体名称
        /// </summary>
        public string GroupInfoName { get; set; }

        /// <summary>
        /// 导游编号<example>11,12,13</example>>
        /// </summary>
        public string GuideIds { get; set; }

        /// <summary>
        /// 司陪<example>11,12,13</example>>
        /// </summary>
        public string DriverIds { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string LicensePlateNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
