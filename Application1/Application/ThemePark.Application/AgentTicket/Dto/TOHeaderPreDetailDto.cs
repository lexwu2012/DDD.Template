using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Application.Order.Dto;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Web.Models;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 预订单详情dto
    /// </summary>
    [AutoMap(typeof(TOHeaderPre))]
    public class TOHeaderPreDetailDto : AuditedModel<string>
    {
        /// <summary>
        /// cotr
        /// </summary>
        public TOHeaderPreDetailDto()
        {
            TOBodyPres = new List<TOBodyPreDetailDto>();
            AgencySaleTicketClassList = new List<AgencySaleTicketClassOrderDto>();
        }

        /// <summary>
        /// 子订单信息
        /// </summary>
        public IList<TOBodyPreDetailDto> TOBodyPres { get; set; }

        /// <summary>
        /// 可选票类
        /// </summary>
        public IList<AgencySaleTicketClassOrderDto> AgencySaleTicketClassList { get; set; }

        /// <summary>
        /// 代理商Id
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 团体类型Id
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 公园代理商团体类型Id（在保存的时候需要用到这个id验证代理商规则）
        /// </summary>
        public int ParkAgencyTypeGroupTypeId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        [MapFrom(nameof(TOHeaderPre.Id))]
        public string TOHeaderPreId { get; set; }

        /// <summary>
        /// 旅行社名称
        /// </summary>
        [MapFrom(nameof(TOHeaderPre.Agency), nameof(Agency.AgencyName))]
        public string AgencyName { get; set; }

        /// <summary>
        /// 预订人数（不包括免票的人数）
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 预订免票人数
        /// </summary>
        public int FreePersons { get; set; }

        /// <summary>
        /// 确认人数（不包括免票的人数）
        /// </summary>
        public int? ConfirmPersons { get; set; }

        /// <summary>
        /// 确认免票人数
        /// </summary>
        public int? ConfirmFreePersons { get; set; }

        /// <summary>
        /// 预订购票数量（不包括免票，预总）
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
        /// 预订总金额
        /// </summary>   
        [MapFrom(nameof(TOHeaderPre.Amount))]
        public decimal SumAmount { get; set; }

        /// <summary>
        /// 确认总额
        /// </summary>
        public decimal? ConfirmAmount { get; set; }

        /// <summary>
        /// 有效开始日期
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
        /// 团队信息
        /// </summary>
        public GroupInfoDto GroupInfo { get; set; }

        /// <summary>
        /// 带队类型名称
        /// </summary>    
        [MapFrom(nameof(TOHeaderPre.GroupType), nameof(GroupType.TypeName))]
        public string GroupTypeName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 订单详情
        /// </summary>
        public OrderDetailDto OrderDetailDto { get; set; }

        /// <summary>
        /// 订单是否已过期（过期将不显示确认编辑等按钮）
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// 导游是否更改
        /// </summary>
        public bool IsGuideModify => false;

        /// <summary>
        /// 司机是否更改
        /// </summary>
        public bool IsDriverModify => false;
    }
}
