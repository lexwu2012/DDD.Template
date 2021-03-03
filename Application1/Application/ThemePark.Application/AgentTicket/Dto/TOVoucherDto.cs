using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    [AutoMapFrom(typeof(TOVoucher))]
    public class TOVoucherDto : FullAuditedEntityDto<string>
    {
        public string SubOrderId { get; set; }
          
        public int Seq { get; set; }

        public long? CustomerId { get; set; }

        public string Pid { get; set; }
          
        public string TicketCode { get; set; }
            
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public virtual CustomerDto Customer { get; set; }

        public virtual TOBodyDto SubOrderTOBody { get; set; }
    }
}
