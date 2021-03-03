using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    [AutoMapTo(typeof(TicketClassDetail))]
    public class GetTicketClassDetailPageInput
    {
        /// <summary>
        /// TicketClassId
        /// </summary>    
        public int? TicketClassId { get; set; }
        /// <summary>
        /// TicketTypeId
        /// </summary>    
        public int? TicketTypeId { get; set; }
        /// <summary>
        /// Qty
        /// </summary>    
        public int? Qty { get; set; }
    }
}

