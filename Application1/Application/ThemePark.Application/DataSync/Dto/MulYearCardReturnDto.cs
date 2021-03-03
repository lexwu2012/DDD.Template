using Abp.Domain.Entities.Auditing;
using System;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.DataSync.Dto
{
    [Serializable]
    public class MulYearCardReturnDto
    {
        public int ParkId { get; set; }

        /// <summary>
        /// 退票金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? CreatorUserId { get; set; }



        public MulVipCardDto VIPCard { get; set; }



        public MulVipCardReturnDto VipCardReturn { get; set; }


        public MulVipVoucherReturnDto VipVoucherReturn { get; set; }
    }


    /// <summary>
    /// 退凭证记录表
    /// </summary>
    public class MulVipVoucherReturnDto : CreationAuditedEntity<long>
    {
        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// VIPVOUCHER id
        /// </summary>

        public long VipVoucherId { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 原交易号
        /// </summary>
        public string OriginalTradeInfoId { get; set; }

        /// <summary>
        /// 退票金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 申请操作人姓名
        /// </summary>
        public string ApplyName { get; set; }

        /// <summary>
        /// 申请操作人证件号
        /// </summary>
        public string ApplyPid { get; set; }

        /// <summary>
        /// 申请操作人手机号
        /// </summary>
        public string ApplyPhone { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }

    /// <summary>
    /// 退卡记录表
    /// </summary>
    public class MulVipCardReturnDto : FullAuditedEntity<long>
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
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 原交易号
        /// </summary>
        public string OriginalTradeInfoId { get; set; }

        /// <summary>
        /// 退票金额
        /// </summary>    

        public decimal Amount { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 申请操作人姓名
        /// </summary>
        public string ApplyName { get; set; }

        /// <summary>
        /// 申请操作人证件号
        /// </summary>
        public string ApplyPid { get; set; }

        /// <summary>
        /// 申请操作人手机号
        /// </summary>
        public string ApplyPhone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }

}
