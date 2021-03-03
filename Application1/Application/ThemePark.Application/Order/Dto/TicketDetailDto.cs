using System;
using System.Collections.Generic;

namespace ThemePark.Application.Order.Dto
{
    /// <summary>
    /// 当前票信息
    /// </summary>
    public class TicketDetailDto
    {
        /// <summary>
        /// cotr
        /// </summary>
        public TicketDetailDto()
        {
            TicketInParkInfos = new List<TicketInParkInfo>();
        }

        /// <summary>
        /// 票的条码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 当前状态名称
        /// </summary>
        public string TicketSaleStatusName { get; set; }

        /// <summary>
        /// OTA特有出票方式(纸质票或电子票)
        /// </summary>
        public string TicketFormEnumName { get; set; }

        /// <summary>
        /// 票数
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 计划入园日期开始的有效天数
        /// </summary>    
        public int ValidDays { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 出票时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 出票操作员
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 退款信息(退款 应该和入园互斥，入园就不能退款了，反之)
        /// </summary>
        public RefundInfo TicketRefundInfo { get; set; }

        /// <summary>
        /// 入园信息(一张票可以有多次入园记录)
        /// </summary>
        public IList<TicketInParkInfo> TicketInParkInfos { get; set; }
    }

    /// <summary>
    /// 入园信息
    /// </summary>
    public class TicketInParkInfo
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 首次入园时间
        /// </summary>
        public DateTime? InParkTime { get; set; }

        /// <summary>
        /// 入园闸口
        /// </summary>
        public string InParkChannel { get; set; }

        /// <summary>
        /// 入园次数
        /// </summary>    
        public int Qty { get; set; }

        /// <summary>
        /// 入园公园
        /// </summary>
        public string ParkName { get; set; }
    }

    /// <summary>
    /// 退款信息
    /// </summary>
    public class RefundInfo
    {
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime RefundTime { get; set; }

        /// <summary>
        /// 退票原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 退款人员
        /// </summary>
        public string CreatorUserName { get; set; }
    }
}
