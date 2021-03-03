using System;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.DataSync.Dto
{
    [Serializable]
    public class MulYearCardVoucherSaleDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// VIP卡ID 兑换VIP卡后更新这个字段
        /// </summary>    
        public long? VIPCardId { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public System.DateTime? ValidDateBegin { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>    
        public System.DateTime? ValidDateEnd { get; set; }

        /// <summary>
        /// 状态
        /// </summary>    
        public VipVoucherStateType State { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>    
        public string Invoice { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }
        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? CreatorUserId { get; set; }

        /// <summary>
        /// 条码号
        /// </summary> 
        public string Barcode { get; set; }


        /// <summary>
        /// 备注
        /// </summary>  
        public string Remark { get; set; }

    }
}
