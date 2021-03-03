using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMap(typeof(OtherNonGroupTicket))]
    public class OtherNonGroupTicketDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get;set;}

        /// <summary>
        /// 基础票类ID
        /// </summary>
        [MapFrom(nameof(OtherNonGroupTicket.ParkSaleTicketClass),nameof(Core.BasicTicketType.ParkSaleTicketClass.TicketClassId))]
        public int TicketClassId { get; set; }

        /// <summary>
        /// 基础票种Id
        /// </summary>
        [MapFrom(nameof(OtherNonGroupTicket.ParkSaleTicketClass), nameof(Core.BasicTicketType.ParkSaleTicketClass.TicketClass),nameof(TicketClass.TicketTypeId))]
        public string TickeTypeId { get; set; }

        /// <summary>
        /// 卖方FromParkId
        /// </summary>    
        public int ParkId { get; set; }

        /// <summary>
        /// 门票类型编号
        /// </summary>    
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 终端号，非本公园出票为空
        /// </summary>    
        public int? TerminalId { get; set; }

        /// <summary>
        /// 门票数量
        /// </summary>    
        public int Qty { get; set; }

        /// <summary>
        /// 门市价格
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>    
        [MapFrom(nameof(OtherNonGroupTicket.ParkSaleTicketClass), nameof(Core.BasicTicketType.ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassName))]
        public string SaleTicketClassName { get; set; }


        /// <summary>
        /// 人数
        /// </summary>    
        [MapFrom(nameof(OtherNonGroupTicket.ParkSaleTicketClass), nameof(Core.BasicTicketType.ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketType), nameof(TicketType.Persons))]
        public int Persons { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(OtherNonGroupTicket.Park), nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 计划开始使用日期
        /// </summary>    
        public System.DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 有效天数
        /// </summary>    
        public int ValidDays { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(512)]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        public virtual ParkSaleTicketClass ParkSaleTicketClass { get; set; }
    }
}
