using Abp.Application.Services.Dto;

namespace ThemePark.Application.AgentTicket.Dto
{
    public class TOMessageDto : FullAuditedEntityDto
    {
        public int? TOHeadId { get; set; }

        public int? CustomId { get; set; }
        /// <summary>
        /// AgentID 谁卖出的订单
        /// </summary>    
        public int? AgentId { get; set; }

        public string Message { get; set; }
        /// <summary>
        /// 游客的手机号
        /// </summary>    
        public string SendTo { get; set; }
   
        public System.DateTime? SendTime { get; set; }
  
        public string Remark { get; set; }
    }
}
