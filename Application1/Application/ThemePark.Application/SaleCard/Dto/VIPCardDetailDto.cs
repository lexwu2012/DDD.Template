using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Common;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡详细信息
    /// </summary>
    [AutoMap(typeof(VIPCard))]
    public   class VIPCardDetailDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 基础票类编号
        /// </summary>    
        public int TicketClassId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public int IcBasicInfoId { get; set; }
        /// <summary>
        /// 帐户编号
        /// </summary>    
        public int? VIPAccountId { get; set; }
        /// <summary>
        /// 卡状态：０预售未激活 １已激活 正常 ２挂失  ３作废
        /// </summary>    
        public VipCardStateType State { get; set; }

        /// <summary>
        /// 数据状态说明
        /// </summary>
        public string StateName => State.DisplayName();
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// IC卡初始化信息
        /// </summary>
        public virtual IcBasicInfoDto IcBasicInfo { get; set; }

        /// <summary>
        /// 年卡卡类信息
        /// </summary>
        public virtual ParkSaleTicketClassDto ParkSaleTicketClass { get; set; }

        /// <summary>
        /// 年卡卡类基础信息
        /// </summary>
        public virtual TicketClassDto TicketClass { get; set; }

        /// <summary>
        /// 年卡客户信息
        /// </summary>
        public virtual List<CustomerInput> Customer { get; set; }
    }
}
