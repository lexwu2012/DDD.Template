using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 电商下单
    /// </summary>
    [AutoMapTo(typeof(TOHeader))]
    public class OrderInput:ValidateParams
    {

        /// <summary>
        /// 第三方代理商订单号
        /// </summary>
        [StringLength(128)]
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 第三方代理商交易号
        /// </summary>
        [StringLength(128)]
        public string AgentTradeNo { get; set; }

        /// <summary>
        /// 订单详情
        /// </summary>
        public virtual ICollection<OTAOrderDetailInput> TOBodies { get; set; }

        /// <summary>
        /// 方特会员编号
        /// </summary>
        public string FangteVipID { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 有效截止日期
        /// </summary>
        public DateTime? ValidEndDate { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 订单类型：OTA订单
        /// </summary>
        public OrderType OrderType => OrderType.OTAOrder;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string EmailServer { get; set; }

    }


}
