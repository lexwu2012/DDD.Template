using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    [AutoMapTo(typeof(ParkTicketStandardPrice))]
    public class GetParkTicketStandardPricePageInput
    {
        public int? Id { get; set; }
        /// <summary>
        /// ParkId
        /// </summary>    
        [Required, Range(1, int.MaxValue)]
        public int ParkId { get; set; }

        /// <summary>
        /// TicketTypeId
        /// </summary>    
        [Required, Range(1, int.MaxValue)]
        public int TicketTypeId { get; set; }

        /// <summary>
        /// StandardPrice
        /// </summary>    
        [Range(1, double.MaxValue)]
        public decimal StandardPrice { get; set; }


        public string TicketTypeName { get; set; }

        public string ParkName { get; set; }

        /// <summary>
        /// 是否软删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}

