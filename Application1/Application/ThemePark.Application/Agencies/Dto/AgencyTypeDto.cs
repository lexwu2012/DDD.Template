using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(AgencyType))]
    public class AgencyTypeDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 代理商类型名称
        /// </summary>
        [DisplayName("代理商类型名称")]
        [Required(ErrorMessage = "代理商类型名称是必填字段")]
        [StringLength(50)]
        public string AgencyTypeName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>   
        [DisplayName("备注")]
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 默认代理商类型：其他、旅行社、OTA
        /// </summary>
        [Required(ErrorMessage = "请选择默认代理商类型")]
        public DefaultAgencyType DefaultAgencyType { get; set; }

        public OutTicketType OutTicketType { get; set; }

        public string OutTicketTypeName => OutTicketType.DisplayName();

        /// <summary>
        /// 默认排序
        /// </summary>
        [Range(0, 255)]
        [DisplayName("默认排序")]
        public int SerialNumber { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public virtual List<GetParkAgencyTypeGroupTypeDto> AgencyTypeGroupTypes { get; set; }

    }

}
