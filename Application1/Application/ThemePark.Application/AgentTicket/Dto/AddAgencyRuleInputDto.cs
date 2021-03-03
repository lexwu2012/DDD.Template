using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// input
    /// </summary>
    [AutoMap(typeof(AgencyRule))]
    public class AgencyRuleInput
    {
        /// <summary>
        /// 规则名称
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        [DisplayName("规则名称"), StringLength(50)]
        public string AgencyRuleName { get; set; }
        /// <summary>
        /// 最小团体人数
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        [Range(0, int.MaxValue)]
        [DisplayName("最小团体人数")]
        public int MinQty { get; set; }
        /// <summary>
        /// 最大团体人数
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        [Range(0, int.MaxValue)]
        [DisplayName("最大团体人数")]
        public int MaxQty { get; set; }
        /// <summary>
        /// 是否需要订单确认
        /// </summary>    
        [DisplayName("是否需要订单确认")]
        [Required]
        public bool IsNeedConfitmOrder { get; set; }
        /// <summary>
        /// 是否可订单支付
        /// </summary>  
        [DisplayName("是否可订单支付")]
        [Required]
        public bool IsCanPayOrder { get; set; }
        /// <summary>
        /// 是否必须身份证
        /// </summary>    
        [DisplayName("是否必须身份证")]
        [Required]
        public bool IsNeedIdnum { get; set; }
        /// <summary>
        /// 每人（每张身份证）限购票数 每人/每天/每单个渠道
        /// </summary>    
        [DisplayName("每张身份证购票数")]
        [Range(0, int.MaxValue)]
        public int? IdnumTicketQty { get; set; }
        /// <summary>
        /// 是否身份证入园
        /// </summary>  
        [DisplayName("是否身份证入园")]
        [Required]
        public bool CanPidInpark { get; set; }
        /// <summary>
        /// 是否二维码入园
        /// </summary>    
        [DisplayName("是否二维码入园")]
        [Required]
        public bool CanQrcodeInpark { get; set; }
        /// <summary>
        /// 是否取票码取票
        /// </summary>    
        [DisplayName("是否取票码取票")]
        [Required]
        public bool CanTicketCodeTaken { get; set; }
    }
}