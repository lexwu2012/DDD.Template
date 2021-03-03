using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 年卡可销售类别
    /// </summary>
    [AutoMap(typeof(ParkSaleTicketClass))]
    public class ParkSaleTicketClassYearCardDto 
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ParkId
        /// </summary>    
        public int ParkId { get; set; }
        /// <summary>
        /// SaleTicketClassName
        /// </summary>    
        public string SaleTicketClassName { get; set; }
        /// <summary>
        /// 系统计算出门市价
        /// </summary>    
        public decimal Price { get; set; }
        /// <summary>
        /// 促销价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 基础票类编号
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 基础票种
        /// </summary>
        [MapFrom(nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketTypeId))]
        public string TicketTypeId { get; set; }

        /// <summary>
        /// 票种顺序
        /// </summary>
        [MapFrom(nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketType),nameof(TicketType.SerialNumber))]
        public int SerialNumber { get; set; }



    }
}
