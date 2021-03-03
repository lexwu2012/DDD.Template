using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 订单更改输入
    /// </summary>
    [AutoMapTo(typeof(TOHeaderPre))]
    public class TOHeaderPreEditInput
    {
        public TOHeaderPreEditInput()
        {
            TOBodyPres = new List<TravelTOBodyEditInput>();
        }

        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 子订单信息
        /// </summary>
        [Required]
        public List<TravelTOBodyEditInput> TOBodyPres { get; set; }

        /// <summary>
        /// 团队人员信息
        /// </summary>
        [Required]
        public GroupInfoEditInput GroupInfo { get; set; }

        /// <summary>
        /// 主订单备注
        /// </summary>
        public string Remark { get; set; }
    }
}
