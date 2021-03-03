using System;

namespace ThemePark.Application.DataSync.Dto
{

    /// <summary>
    /// 多园年卡销售Dto
    /// </summary>
    [Serializable]
    public class MulYearCardSaleDto
    {
        public long VipCardId { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int? ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 基础票类Id
        /// </summary>
        public int TicketClassId { get; set; }


        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long IcBasicInfoId { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public int? TerminalId { get; set; }


        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? CreatorUserId { get; set; }

        /// <summary>
        /// 售卡人
        /// </summary>
        public long? SaleUser { get; set; }

        /// <summary>
        /// 售卡时间
        /// </summary>
        public DateTime? SaleTime { get; set; }

    }
}
