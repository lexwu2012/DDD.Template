using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    [AutoMapFrom(typeof(GroupMemberDto))]
    [AutoMapTo(typeof(GroupMembers))]
    public class GroupMemberInput
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// 客户信息
        /// </summary>
        [Required]
        public CustomerInput Customer { get; set; }
    }
}
