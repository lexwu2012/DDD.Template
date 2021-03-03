using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    [AutoMap(typeof(GroupMembers))]
    public class GroupMemberDto
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        [Required]
        public int CustomerId { get; set; }


        /// <summary>
        /// 团体信息编号
        /// </summary>
        public int GroupInfoId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>The customer.</value>
        public virtual Customer Customer { get; set; }
        
    }
}
