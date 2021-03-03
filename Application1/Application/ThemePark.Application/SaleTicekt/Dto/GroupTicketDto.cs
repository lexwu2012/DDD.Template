using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.InPark;
using ThemePark.Core.ParkSale;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMap(typeof(GroupTicket))]
    public class GroupTicketDto : IEntityDto<string>
    {
        public string Id { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>    
        [Required]
        public int ParkId { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        [Required]
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>    
        [Required]
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 终端号，非本公园出票为空
        /// </summary>    
        public int? TerminalId { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>    
        [Required]
        public int AgencyId { get; set; }

        /// <summary>
        /// 团体类型编号
        /// </summary>    
        //ToDo: Needed?
        //delete
        [Required]
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 主订单编号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 子订单Id
        /// </summary>
        public string TOBodyId { get; set; }

        /// <summary>
        /// 门票数量
        /// </summary>    
        [Required]
        public int Qty { get; set; }

        /// <summary>
        /// 门市价格
        /// </summary>    
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// 实际价格
        /// </summary>    
        [Required]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 金额
        /// </summary>    
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        [Required]
        public System.DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 计划入园日期开始的有效天数
        /// </summary>    
        [Required]
        public int ValidDays { get; set; }

        /// <summary>
        /// 入园总次数
        /// </summary>
        public int InparkCounts { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>    
        public long? InvoiceId { get; set; }

        /// <summary>
        /// 门票状态
        /// </summary>    
        [Required]
        public TicketSaleStatus TicketSaleStatus { get; set; }

        /// <summary>
        /// 同步票类型
        /// </summary>
        public SyncTicketType? SyncTicketType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(512)]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual GroupType GroupType { get; set; }
        public virtual Park Park { get; set; }
        public virtual ParkSaleTicketClass ParkSaleTicketClass { get; set; }
        public virtual AgencySaleTicketClass AgencySaleTicketClass { get; set; }
        public virtual Terminal Terminal { get; set; }
        public virtual TradeInfo TradeInfo { get; set; }
        public virtual ICollection<TicketInPark> TicketInParks { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual TOHeader TOHeader { get; set; }
        public virtual TOBody TOBody { get; set; }
    }
}