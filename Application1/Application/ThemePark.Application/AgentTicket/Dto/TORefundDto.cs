using Abp.Application.Services.Dto;

namespace ThemePark.Application.AgentTicket.Dto
{
    public class TORefundDto : FullAuditedEntityDto
    {
        public int TOHeaderId { get; set; }

        public int TOModifyId { get; set; }
  
        public decimal Amount { get; set; }
  
        public string Reason { get; set; }

        public string Remark { get; set; }
        /// <summary>
        /// 退款交易号
        /// </summary>    
        public string TradeInfoId { get; set; }
    }
}
