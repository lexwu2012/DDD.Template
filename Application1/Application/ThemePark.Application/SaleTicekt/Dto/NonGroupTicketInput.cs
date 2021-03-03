using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.ParkSale;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 散客购票详情
    /// </summary>
    [AutoMapTo(typeof(NonGroupTicket))]
    public class NonGroupTicketInput : ITicketInfo
    {
        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 促销票类ID
        /// </summary>
        [Required]
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 门票数量
        /// </summary>    
        [Required]
        public int Qty { get; set; }

        /// <summary>
        /// 起始有效期
        /// </summary>
        [Required]
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 总票输出、分票输出
        /// </summary>
        public int IsAllOutPut { get; set; }

        /// <summary>
        /// 注意这里的TicketClassId并不是TicketClass表里的ID，是取自ParkSaleTicketClassId！
        /// </summary>
        public int TicketClassId => ParkSaleTicketClassId;

        /// <summary>
        /// 
        /// </summary>
        public TicketClassMode TicketClassMode { get; set; }
    }
}
