using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 代理商购票 新增取票记录 输入dto
    /// </summary>
    [AutoMapTo(typeof(TOTicket))]
    public class ToTicketInput
    {

        /// <summary>
        /// 票所属公园编号
        /// </summary>    
        [Required]
        public int ParkId { get; set; }

        /// <summary>
        /// 取票所在公园编号
        /// </summary>
        public int FromParkId { get; set; }

        /// <summary>
        /// 取票凭证编号
        /// </summary>    
        [Required]
        public List<string> TOBodyIds { get; set; }
  
        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }


        /// <summary>
        /// 发票信息
        /// </summary>
        public InvoiceInput InvoiceInfos { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        public long? CreatorUserId { get; set; }


        public ToTicketInput ()
        {
            TOBodyIds = new List<string>();
        }



    }
}
