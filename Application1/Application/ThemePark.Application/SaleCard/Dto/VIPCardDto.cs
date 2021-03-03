using Abp.AutoMapper;
using System;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Common;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 会员卡信息
    /// </summary>
    [AutoMap(typeof(VIPCard))]
    public  class VIPCardDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int? ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 票类ID
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long  IcBasicInfoId { get; set; }

        /// <summary>
        /// 帐户编号
        /// </summary>    
        public long? VIPAccountId { get; set; }

        /// <summary>
        /// 卡状态：０预售未激活 １已激活 正常 ２挂失  ３作废
        /// </summary>    
        public VipCardStateType State { get; set; }

        /// <summary>
        /// 数据状态说明
        /// </summary>
        public string StateName => State.DisplayName();

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public DateTime? ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public DateTime? ValidDateEnd { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// IC卡初始化信息
        /// </summary>
        public virtual IcBasicInfoDto IcBasicInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public virtual TicketClassDto TicketClass { get; set; }

    }
}
