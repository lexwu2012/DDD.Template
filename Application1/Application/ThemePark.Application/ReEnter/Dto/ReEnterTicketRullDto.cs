using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.ReEnter;

namespace ThemePark.Application.ReEnter.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(ReEnterTicketRull))]
    public class ReEnterTicketRullDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 二次入园规则编号
        /// </summary>    
        public int ReEnterEnrollRuleId { get; set; }

        /// <summary>
        /// 票类编号
        /// </summary>    
        public int TicketClassId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }

        public virtual ReEnterEnrollRuleDto ReEnterEnrollRule { get; set; }
        public virtual TicketClassDto TicketClass { get; set; }
    }
}
