using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 添加旅行社预订信息
    /// </summary>
    [AutoMap(typeof(TOHeaderPre))]
    public class AddAgencyReserveInput
    {
        /// <summary>
        /// cotr
        /// </summary>
        public AddAgencyReserveInput()
        {
            TOBodyPres = new List<TravelTOBodyInput>();
        }

        /// <summary>
        /// 代理商编号
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 带队类型编号
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 入园日期
        /// </summary>    
        [Required]
        public DateTime ValidStartDate { get; set; }               

        /// <summary>
        /// 团队人员信息
        /// </summary>
        [Required]
        public GroupInfoInput GroupInfo { get; set; }

        /// <summary>
        /// 子订单预订信息
        /// </summary>
        [Required]
        public List<TravelTOBodyInput> TOBodyPres { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
