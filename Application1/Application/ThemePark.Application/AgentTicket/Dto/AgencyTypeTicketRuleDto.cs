using Abp.Application.Services.Dto;

namespace ThemePark.Application.AgentTicket.Dto
{
    public class AgencyTypeTicketRuleDto : FullAuditedEntityDto
    {
        public string RuleName { get; set; }

        public int MinQty { get; set; }
  
        public int MaxQty { get; set; }
 
        public string IsNeedConfitmOrder { get; set; }

        public int IsCanPayOrder { get; set; }
  
        public string IsNeedIdnum { get; set; }
        /// <summary>
        /// 每人（每张身份证）限购票数 没人/每天/每单个渠道
        /// </summary>    
        public int? IdnumTicketQty { get; set; }
        /// <summary>
        /// 是否身份证入园
        /// </summary>    
        public bool? CanPidInpark { get; set; }
   
        public bool? CanQrcodeInpark { get; set; }
        /// <summary>
        /// 是否取票码取票
        /// </summary>    
        public bool? CanTicketCodeTaken { get; set; }
    }
}
