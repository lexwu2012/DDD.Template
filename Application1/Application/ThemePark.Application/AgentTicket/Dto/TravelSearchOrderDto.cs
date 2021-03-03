using System;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 搜索旅行社订单信息dto
    /// </summary>
    public class TravelSearchOrderDto : PageQuery<TOHeaderPre>
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Query(QueryCompare.Like, nameof(TOHeaderPre.Id))]
        public string Id { get; set; }

        /// <summary>
        /// 预订公园Id
        /// </summary>
        public int? ParkId { get; set; }

        /// <summary>
        /// 带队类型
        /// </summary>
        [Query]
        public int? GroupTypeId { get; set; }

        /// <summary>
        /// 入园时间开始时间
        /// </summary>
        [Query(QueryCompare.GreaterThanOrEqual, nameof(TOHeaderPre.ValidStartDate))]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 入园时间结束时间
        /// </summary>
        [Query(QueryCompare.LessThanOrEqual, nameof(TOHeaderPre.ValidStartDate))]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderState? OrderState { get; set; }

        /// <summary>
        /// 主订单状态
        /// </summary>
        [Query]
        public MainOrderState? MainOrderState { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [Query(QueryCompare.Like, nameof(TOHeaderPre.Agency), nameof(Core.Agencies.Agency.AgencyName))]
        public string AgencyName { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [Query]
        public string AgencyId { get; set; }

        /// <summary>
        /// 下单时间查询起始日期
        /// </summary>
        [Query(QueryCompare.GreaterThanOrEqual, nameof(TOHeader.CreationTime))]
        public DateTime? CreationTimeBegin { get; set; }

        /// <summary>
        /// 下单时间查询结束日期
        /// </summary>
        [Query(QueryCompare.LessThanOrEqual, nameof(TOHeader.CreationTime))]
        public DateTime? CreationTimeEnd { get; set; }

        /// <summary>
        /// draw count, return to client
        /// </summary>
        public int Draw { get; set; }

    }
}
