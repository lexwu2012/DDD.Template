using System;

namespace ThemePark.Application.DataSync.Dto
{
    [Serializable]
    public class MulYearCardVoucherReturnDto
    {
        public long Id { get; set; }
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
        /// 
        /// </summary>
        public long? CreatorUserId { get; set; }

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
