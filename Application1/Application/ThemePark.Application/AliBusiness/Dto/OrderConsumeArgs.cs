using System;

namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 核销参数
    /// </summary>
    [Serializable]
    public class OrderConsumeArgs
    {
        /// <summary>
        /// 返回的取票码
        /// </summary>
        public string Ticketcode { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string Idnum { get; set; }

        /// <summary>
        /// 票类
        /// </summary>
        public string Tickettypeid { get; set; }

        /// <summary>
        ///  天猫订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrganizerNick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ParkId { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string MainOrderId { get; set; }

        /// <summary>
        /// 商品目录
        /// </summary>
        public string ItemTitle { get; set; }

        /// <summary>
        /// 方特订单Id
        /// </summary>
        public string FtOrderId { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderCreationTime { get; set; }
    }
}
