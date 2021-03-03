using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    [AutoMap(typeof(ParkTicketStandardPrice))]
    public class ParkTicketStandardPriceDto : FullAuditedEntityDto
    {
        /// <summary>
        /// ParkId
        /// </summary>    
        public int ParkId { get; set; }
        public string ParkName { get; set; }

        public string ParkCode { get; set; }

        /// <summary>
        /// TicketTypeId
        /// </summary>    
        public int TicketTypeId { get; set; }

        public string TicketTypeCode { get; set; }

        public string TicketTypeName { get; set; }

        /// <summary>
        /// StandardPrice
        /// </summary>    
        public decimal StandardPrice { get; set; }

        public string Remark { get; set; }

        public ParkDto Park { get; set; }
        public TicketTypeDto TicketType { get; set; }

    }
}

