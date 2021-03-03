using System;
using Abp.AutoMapper;
using ThemePark.Core.ParkSale;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapFrom(typeof(NonGroupTicket), typeof(GroupTicket))]
    public class GetSaleDetailDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeinfoId { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// 交易方式
        /// </summary>
        public string PayMode { get; set; }

        /// <summary>
        /// 交易方式
        /// </summary>
        public PayType PayModeId { get; set; }

        /// <summary>
        /// 门票数量
        /// </summary>    
        public int Qty { get; set; }

        /// <summary>
        /// 实际价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 门票状态
        /// </summary>    
        public string StateDisplayName { get; set; }


        /// <summary>
        /// 销售时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

    }

}
