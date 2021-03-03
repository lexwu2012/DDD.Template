using System;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 套票同步Dto
    /// </summary>
    [Serializable]
    public class MultiTicketSendDto
    {
        /// <summary>
        /// 票类型 团体、散客
        /// </summary>
        public TicketCategory TicketClassType { get; set; }

        /// <summary>
        /// 来源公园
        /// </summary>
        public int FromParkid { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 票类编号
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>    
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 票数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        public decimal? ParkSettlementPrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal? SettlementPrice { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 总金额
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
        /// 入园总次数
        /// </summary>
        public int InparkCounts { get; set; }


        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>    
        public int AgencyId { get; set; }

        /// <summary>
        /// 团体类型编号
        /// </summary>    
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 取票凭证编号
        /// </summary>
        public string TOVoucherId { get; set; }

        /// <summary>
        /// 新增人员
        /// </summary>
        public long? CreatorUserId { get; set; }



        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string TOHeaderId { get; set; }


        /// <summary>
        /// 同步票类型
        /// </summary>
        public SyncTicketType? SyncTicketType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime { get; set; }

        public  DateTime? LastModificationTime { get; set; }

        public  long? LastModifierUserId { get; set; }


        public TicketFormEnum TicketFormEnum { get; set; }

        /// <summary>
        /// 子订单Id
        /// </summary>
        public string TOBodyId { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>    
        public long? InvoiceId { get; set; }

        /// <summary>
        /// 票持有人
        /// </summary>    
        public int? CustomerId { get; set; }

    }
}
