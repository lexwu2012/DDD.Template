using Abp.Application.Services.Dto;

namespace ThemePark.Application.AgentTicket.Dto
{
    public class TOModifyDto : FullAuditedEntityDto
    {
        public int TOHeadId { get; set; }
        /// <summary>
        /// 订单修改的内容
        /// </summary>    
        public string OperContent { get; set; }
 
        public string Remark { get; set; }
    }
}
