using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(ParkAgency))]
    public class ParkAgencyDto: FullAuditedEntityDto
    {
        /// <summary>
        /// 公园编号
        /// </summary>    
        [Required]
        public int ParkId { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>    
        [Required]
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商类型编号
        /// </summary>    
        [Required]
        public int AgencyTypeId { get; set; }

        /// <summary>
        /// 0公园 1国旅(结算对象)
        /// </summary>
        [Required]
        public SettlementType Settlement { get; set; }
      
        /// <summary>
        /// 政策协议
        /// </summary>    
        [StringLength(100)]
        public string PolicyNote { get; set; }

        /// <summary>
        /// 票务协议
        /// </summary>    
        [StringLength(50)]
        public string TicketNote { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 到期日期
        /// </summary>
        public DateTime ExpirationDateTime { get; set; }

        /// <summary>
        /// 许可证
        /// </summary>    
        [StringLength(50)]
        public string Licence { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 关联代理商
        /// </summary>
        public virtual AgencyDto Agency { get; set; }

        /// <summary>
        /// 关联的代理商类型
        /// </summary>
        public virtual AgencyTypeDto AgencyType { get; set; }

        /// <summary>
        /// 关联公园
        /// </summary>
        public virtual ParkDto Park { get; set; }
    }
}
