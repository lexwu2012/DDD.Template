using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 网络取票input
    /// </summary>
    public class SaveWebTicketInput
    {
        /// <summary>
        /// 取票订单号
        /// </summary>
        [Required]
        public List<GetOrderDetailInput> OrderDetailInputs;

        /// <summary>
        /// 发票信息
        /// </summary>
        [Required]
        public InvoiceInput InvoiceInfos { get; set; }
    }
}
