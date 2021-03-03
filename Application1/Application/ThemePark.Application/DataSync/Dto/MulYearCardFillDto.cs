using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 多园年卡补卡Dto
    /// </summary>
    [Serializable]
    public class MulYearCardFillDto
    {
        public int ParkId { get; set; }

        public string TradeInfoId { get; set; }

        public MulVipCardDto NewVIPCard { get; set; }

        public MulVipCardDto OldVIPCard { get; set; }

        public MulIcoperDetailDto IcoperDetail { get; set; }

        public MulFillCardDto FillCard { get; set; }

    }


    /// <summary>
    /// 补卡记录表
    /// </summary>
    public class MulFillCardDto : FullAuditedEntity<long>
    {
        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// VIPCARD id
        /// </summary>

        public long VipCardId { get; set; }

        /// <summary>
        /// 旧卡卡号
        /// </summary>

        public string OldIcNo { get; set; }

        /// <summary>
        /// 新卡卡号
        /// </summary>

        public string NewIcNo { get; set; }

        /// <summary>
        /// 补卡类型
        /// </summary>
        public FillCardType State { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }
        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>

        public string Remark { get; set; }


    }


    [AutoMap(typeof(VIPCard))]
    public class MulVipCardDto
    {
        public long  Id { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public int? LastModifierUserId { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int? ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 基础票类Id
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long IcBasicInfoId { get; set; }

        /// <summary>
        /// 帐户编号
        /// </summary>    
        public long? VIPAccountId { get; set; }

        /// <summary>
        /// 卡状态：0初始化 1预售未激活  2已激活 正常 3挂失  4作废
        /// </summary>    
        public VipCardStateType State { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }
        /// <summary>
        /// 终端号GetUsersRoleNamesAsync
        /// </summary>    
        public int? TerminalId { get; set; }


        /// <summary>
        /// 售卡人
        /// </summary>
        public long? SaleUser { get; set; }

        /// <summary>
        /// 售卡时间
        /// </summary>
        public DateTime? SaleTime { get; set; }


        /// <summary>
        /// 激活人
        /// </summary>
        public long? ActiveUser { get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime? ActiveTime { get; set; }



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
    }

}
