using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 预订主订单模型
    /// </summary>
    [AutoMap(typeof(TOHeaderPre))]
    public class TOHeaderPreDto
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [MapFrom(nameof(TOHeaderPre.Id))]
        public string TOHeaderPreId { get; set; }

        /// <summary>
        /// 预订总金额
        /// </summary>    
        [MapFrom(nameof(TOHeaderPre.Amount))]
        public decimal SumAmount { get; set; }

        /// <summary>
        /// 预订人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 确认人数
        /// </summary>
        public int? ConfirmPersons { get; set; }

        /// <summary>
        /// 预订购票总数量（预总）
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 免票数量（预免）
        /// </summary>
        public int FreeQty { get; set; }

        /// <summary>
        /// 确认票数（不包括免票的数量，确总）
        /// </summary>
        public int? ConfirmQty { get; set; }

        /// <summary>
        /// 确认免票的数量（确免）
        /// </summary>
        public int? ConfirmFreeQty { get; set; }

        /// <summary>
        /// 预订公园
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 团体名称
        /// </summary>
        [MapFrom(nameof(TOHeaderPre.GroupInfo),nameof(GroupInfo.GroupInfoName))]
        public string GroupInfoName { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [MapFrom(nameof(TOHeaderPre.GroupInfo), nameof(GroupInfo.LicensePlateNumber))]
        public string LicensePlateNumber { get; set; }
        
        /// <summary>
        /// 代理商名称
        /// </summary>
        [MapFrom(nameof(TOHeaderPre.Agency), nameof(Core.Agencies.Agency.AgencyName))]
        public string AgencyName { get; set; }

        /// <summary>
        /// 有效开始日期（入园日期）
        /// </summary>    
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 主订单状态
        /// </summary>
        public MainOrderState MainOrderState { get; set; }

        /// <summary>
        /// 主订单状态名称
        /// </summary>
        public string MainOrderStateName => MainOrderState.DisplayName();

        /// <summary>
        /// 订单类型:旅行社订单/OTA订单
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 带队类型名称
        /// </summary>    
        [MapFrom(nameof(TOHeaderPre.GroupType), nameof(Core.Agencies.GroupType.TypeName))]
        public string GroupTypeName { get; set; }

        /// <summary>
        /// 子订单信息
        /// </summary>
        public IList<TOBodyPreDto> TOBodyPres { get; set; }

        /// <summary>
        /// 是否已核销
        /// </summary>
        public bool IsConsume { get; set; }

        /// <summary>
        /// 订单是否已过期（已过期不会出现确认按钮，因为确认了在窗口也因为入园规则验证而取不到票）
        /// </summary>
        public bool IsExpired { get; set; }
    }
}
