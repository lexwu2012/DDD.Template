using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Application.Users.Dto;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Common;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// AddAgencyInput
    /// </summary>
    [AutoMapTo(typeof(Core.Agencies.Agency))]
    public class UpdateAgencyInput
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>    
        [Required]
        [StringLength(50)]
        public string AgencyName { get; set; }        

        /// <summary>
        /// 城市编号
        /// </summary>    
        [Required]
        public int AddressId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>    
        [StringLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>    
        [StringLength(20)]
        public string Mobile { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }

        //public DefaultAgencyType DefaultAgencyType => DefaultAgencyType.Ota;

        /// <summary>
        /// （OTA）许可主机IP
        /// </summary>
        //[RequiredIf(nameof(DefaultAgencyType), DefaultAgencyType.Ota)]
        public string HostIPs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WeChatNo { get; set; }

        /// <summary>
        /// 账户是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 修改地址
        /// </summary>
        public UpdateAddressInput UpdateAddressInput { get; set; }

        /// <summary>
        /// AgencyUser
        /// </summary>
        public UpdateUserInput UpdateUserInput { get; set; }

    }
}