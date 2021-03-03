using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.AutoMapper;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 同步他园票
    /// </summary>
    [AutoMapTo(typeof(NonGroupTicket))]
    [AutoMapFrom(typeof(OtherNonGroupTicket))]
    public class OtherTicketSendDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 卖方FromParkId
        /// </summary>    
        [Required]
        public int ParkId { get; set; }


        /// <summary>
        /// 终端号，非本公园出票为空
        /// </summary>    
        public int? TerminalId { get; set; }

        /// <summary>
        /// 门票类型编号
        /// </summary>    
        [Required]
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId;

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
        /// 销售价格
        /// </summary>    
        [Required]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 金额
        /// </summary>    
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 计划开始使用日期
        /// </summary>    
        [Required]
        public System.DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 有效天数
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
        /// 票持有人
        /// </summary>    
        public int? CustomerId { get; set; }

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

        /// <summary>
        /// 创建用户
        /// </summary>
        public long CreatorUserId { get; set; }

    }
}
