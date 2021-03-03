using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.ApplicationDto.BasicTicketType
{
    [AutoMapFrom(typeof(TicketClassDetail))]
    public class TicketClassDetailDto : FullAuditedEntityDto
    {
        /// <summary>
        /// TicketClassId
        /// </summary>    
        public int TicketClassId { get; set; }
        /// <summary>
        /// TicketTypeId
        /// </summary>    
        public int TicketTypeId { get; set; }
        /// <summary>
        /// Qty
        /// </summary>    
        public int Qty { get; set; }
        
        public TicketTypeDto TicketType { get; set; }

    }
}

