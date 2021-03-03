using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// AgencyDto
    /// </summary>
    [AutoMap(typeof(Agency))]
    public class AgencyDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 代理商名称
        /// </summary>    
        public string AgencyName { get; set; }

        /// <summary>
        /// 上级代理商编号
        /// </summary>    
        public int? ParentAgencyId { get; set; }

        /// <summary>
        /// 城市编号
        /// </summary>    
        public int AddressId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>    
        public string Email { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>    
        public string Tel { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>    
        public string Mobile { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        public AddressDto Address { get; set; }

        public AgencyDto ParentAgency { get; set; }

        /// <summary>
        /// Gets or sets the agency users.
        /// </summary>
        /// <value>The agency users.</value>
        public AgencyUser AgencyUser { get; set; }

        /// <summary>
        /// 代理商类型
        /// </summary>
        public AgencyTypeDto AgencyType { get; set; }
    }
}