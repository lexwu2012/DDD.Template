using System;
using ThemePark.Core.AliPartner;

namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OrderSendToFtDataArgs
    {
        /// <summary>
        /// 下单数据
        /// </summary>
        public PlaceFtOrderEntity PlaceFtOrderEntity { get; set; }

        /// <summary>
        /// 在job中需要在本地仓储的阿里订单
        /// </summary>
        public TmallOrder TmallOrder { get; set; }

        /// <summary>
        /// 在job中需要在本地仓储的阿里子订单
        /// </summary>
        public TmallOrderDetail TmallOrderDetail { get; set; }
    }

    /// <summary>
    /// 方特系统下单数据
    /// </summary>
    [Serializable]
    public class PlaceFtOrderEntity
    {
        /// <summary>
        /// 下单详情
        /// </summary>
        public FtOrderDetailDto TicketList { get; set; }

        /// <summary>
        /// 方特系统的token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 公园Id
        /// </summary>
        public string Parkid { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string Plandate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string Enddate { get; set; }

        /// <summary>
        /// 下单电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 总额
        /// </summary>
        public string Totalmoney { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string Paymodename { get; set; }

        /// <summary>
        /// 子订单号
        /// </summary>
        public string Outerorderid { get; set; }

        /// <summary>
        /// 付款日期
        /// </summary>
        public string Paydate { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string Tradeno { get; set; }
    }
}
