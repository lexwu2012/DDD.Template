using System;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// OTA订单查询模型
    /// </summary>
    public class OtaSearchOrderDto: PageQuery<TOHeader>
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Query(QueryCompare.Like, nameof(TOHeader.Id))]
        public string Id { get; set; }

        /// <summary>
        /// 入园时间开始时间
        /// </summary>
        [Query(QueryCompare.GreaterThanOrEqual, nameof(TOHeader.ValidStartDate))]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 入园时间结束时间
        /// </summary>
        [Query(QueryCompare.LessThanOrEqual, nameof(TOHeader.ValidStartDate))]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [Query]
        public int? AgencyId { get; set; }

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
        /// 子订单状态
        /// </summary>        
        public OrderState? State { get; set; }

        /// <summary>
        /// 第三方代理商订单号
        /// </summary>
        [Query]
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
    }
}
