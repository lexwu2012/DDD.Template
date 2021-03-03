using Abp.Application.Services.Dto;

namespace ThemePark.Application.AgentTicket.Dto
{
    public class TravelOrderConfirmDto : FullAuditedEntityDto
    {
        public int TOHeadId { get; set; }

        public int ComfirmBy { get; set; }
   
        public string Remark { get; set; }
    }
}
