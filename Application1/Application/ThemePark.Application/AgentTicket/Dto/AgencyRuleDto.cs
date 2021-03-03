using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 代理商规则
    /// </summary>
    [AutoMap(typeof(AgencyRule))]
    public class AgencyRuleOutputDto:FullAuditedEntityDto
    {
        /// <summary>
        /// 规则名称
        /// </summary>    
        [Required]
        [StringLength(50)]
        public string AgencyRuleName { get; set; }
        /// <summary>
        /// 最小团体人数
        /// </summary>    
        [Required]
        public int MinQty { get; set; }
        /// <summary>
        /// 最大团体人数
        /// </summary>    
        [Required]
        public int MaxQty { get; set; }
        /// <summary>
        /// 是否需要订单确认
        /// </summary>    
        [Required]
        public bool IsNeedConfitmOrder { get; set; }
        /// <summary>
        /// 是否可订单支付
        /// </summary>  
        [Required]
        public bool IsCanPayOrder { get; set; }
        /// <summary>
        /// 是否必须身份证
        /// </summary>    
        [Required]
        public bool IsNeedIdnum { get; set; }
        /// <summary>
        /// 每人（每张身份证）限购票数 每人/每天/每单个渠道
        /// </summary>    
        public int? IdnumTicketQty { get; set; }
        /// <summary>
        /// 是否身份证入园
        /// </summary>  
        [Required]
        public bool CanPidInpark { get; set; }
        /// <summary>
        /// 是否二维码入园
        /// </summary>    
        [Required]
        public bool CanQrcodeInpark { get; set; }
        /// <summary>
        /// 是否取票码取票
        /// </summary>    
        [Required]
        public bool CanTicketCodeTaken { get; set; }
    }
}
