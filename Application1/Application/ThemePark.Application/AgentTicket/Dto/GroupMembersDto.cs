using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    [AutoMap(typeof(GroupMembers))]
    public class GroupMembersDto : FullAuditedEntityDto
    {
        public int GroupInfoId { get; set; }

        public int CustomId { get; set; }
  
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>The customer.</value>
        public virtual Customer Customer { get; set; }
    }
}
