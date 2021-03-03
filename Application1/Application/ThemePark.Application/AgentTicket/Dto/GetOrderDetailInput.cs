using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 查询订单详情输入
    /// </summary>
    public class GetOrderDetailInput
    {
        /// <summary>
        ///  公园ID
        /// </summary>
        [Required]
        public int ParkId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Required]
        public string OrderId { get; set; }
    }
}
