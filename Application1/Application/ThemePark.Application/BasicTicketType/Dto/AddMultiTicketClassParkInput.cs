using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    [AutoMapTo(typeof(MultiTicketClassPark))]
    public class AddMultiTicketClassParkInput
    {
        /// <summary>
        /// 公园编号
        /// </summary>
        [Required]
        public int ParkId { get; set; }
        /// <summary>
        /// 票类编号
        /// </summary>
        [Required]
        public int TicketClassId { get; set; }
    }
}
