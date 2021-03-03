using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Trade.Dto;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 旅行社订单头Dto
    /// </summary>
    [AutoMap(typeof(TOHeader))]
    public class TOHeaderDto : FullAuditedEntityDto<string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TOHeaderDto()
        {
            TOBodies = new List<TOBodyDto>();
        }

        /// <summary>
        /// 代理商编号
        /// </summary>    
        public int AgencyId { get; set; }
        /// <summary>
        /// 带队类型编号
        /// </summary>
        public int GroupTypeId { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>    
        public decimal Amount { get; set; }
        /// <summary>
        /// 购票总数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 有效开始日期
        /// </summary>    
        public DateTime ValidStartDate { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>    
        public string TradeInfoId { get; set; }
        /// <summary>
        /// 团体编号
        /// </summary>    
        public int? GroupInfoId { get; set; }
        /// <summary>
        /// 第三方代理商交易号
        /// </summary>    
        public string AgentTradeNo { get; set; }
        /// <summary>
        /// 第三方代理商订单号
        /// </summary>    
        public string AgentOrderId { get; set; }

        ///// <summary>
        ///// 订单状态：0 　待确认　１　待支付　２　待消费  3　订单交易成功 4 订单交易取消 5 订单已退款 6 订单部分退款
        ///// 7 订单交易关闭
        ///// </summary>    
        //public OrderState OrderState { get; set; }

        /// <summary>
        /// 订单类型:旅行社订单/OTA订单
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 子订单
        /// </summary>
        public IList<TOBodyDto> TOBodies;

        /// <summary>
        /// 交易记录
        /// </summary>
        public TradeInfoDto TradeInfo;

        /// <summary>
        /// 团队信息
        /// </summary>
        public GroupInfoDto GroupInfo;

        /// <summary>
        /// 团队类型
        /// </summary>
        public GroupTypeDto GroupType;

        /// <summary>
        /// 旅行社信息
        /// </summary>
        public AgencyOutput Agency { get; set; }
    }

    
}
