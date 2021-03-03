using System;
using System.Collections.Generic;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.Order.Dto
{
    /// <summary>
    /// 订单详情dto
    /// toheader -> tobody（tovoucher） -> ticket -> inpark
    ///                              -> refund
    /// </summary>
    public class OrderDetailDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderDetailDto()
        {
            TOBodyDtosList = new List<TOBodyDetailDto>();
            TOBodyPreDtosList = new List<TOBodyPreDetailDto>();
        }       

        /// <summary>
        /// 订单类型（OTA、旅行社订单）
        /// </summary>
        public string OrderTypeName { get; set; }

        /// <summary>
        /// 订单类型（OTA，旅行社）
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 主订单状态
        /// </summary>
        public MainOrderState MainOrderState { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 子订单号
        /// </summary>
        public IList<TOBodyDetailDto> TOBodyDtosList { get; set; }

        /// <summary>
        /// 预订单号（如果有TOHeaderId，则不需要预订单号，两个互斥）
        /// </summary>
        public string TOHeaderPreId { get; set; }

        /// <summary>
        /// 预订子订单（和TOHeaderPreId配对）
        /// </summary>
        public IList<TOBodyPreDetailDto> TOBodyPreDtosList { get; set; }

        /// <summary>
        /// 预订公园
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 确认总金额
        /// </summary>
        public decimal? ConfirmAmount { get; set; }

        /// <summary>
        /// 确认票的总数量
        /// </summary>
        public int? ConfirmQty { get; set; }

        /// <summary>
        /// 确免票数
        /// </summary>
        public int? ConfirmFreeQty { get; set; }

        /// <summary>
        /// 确认总人数
        /// </summary>
        public int? ConfirmPersons { get; set; }

        /// <summary>
        /// 确免人数
        /// </summary>
        public int? ConfirmFreePerson { get; set; }

        /// <summary>
        /// 主订单下单时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
