using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.AgentTicket.Dto
{
    [AutoMap(typeof(TOTicket))]
    public class TOTicketDto : FullAuditedEntityDto<string>
    {
        public string Barcode { get; set; }
        /// <summary>
        /// 取票凭证ID
        /// </summary>    
        public string TOVoucherId { get; set; }

        public int? TerminalId { get; set; }

        /// <summary>
        /// 0 无效 1 有效 2 已入园 3 已退 4 已作废
        /// </summary>    
        public TicketSaleStatus TicketSaleStatus { get; set; }

        public string Remark { get; set; }
    }
}
