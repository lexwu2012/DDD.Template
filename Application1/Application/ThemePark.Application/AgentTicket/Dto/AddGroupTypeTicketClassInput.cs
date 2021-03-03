using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 添加团队可售票类Input
    /// </summary>
    [AutoMap(typeof(GroupTypeTicketClass))]
    public class AddGroupTypeTicketClassInput
    {
        /// <summary>
        /// 公园Id
        /// </summary>
        [DisplayName("公园：")]
        [Required(ErrorMessage = "公园为必填项")]
        public int ParkId { get; set; }

        /// <summary>
        /// 团体ID
        /// </summary>
        [DisplayName("团体类型：")]
        [Required(ErrorMessage = "团体类型为必填项")]
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 基础票类ID
        /// </summary>
        [DisplayName("可售票类型：")]
        [Required(ErrorMessage = "可售票类型为必填项")]
        public int TicketClassId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50,ErrorMessage ="备注请少于50字")]
        public string Remark { get; set; }
    }
}
