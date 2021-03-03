using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 编辑团队可售票类Input
    /// </summary>
    [AutoMap(typeof(GroupTypeTicketClass))]
    public class GroupTypeTicketClassUpdateInput
    {
        /// <summary>
        /// 基础票类ID
        /// </summary>
        [DisplayName("可售票类型：")]
        [Required(ErrorMessage = "可售票类型为必填项")]
        public IList<int> TicketClassId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50, ErrorMessage = "备注请少于50字")]
        public string Remark { get; set; }
    }
}
