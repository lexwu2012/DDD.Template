using Abp.AutoMapper;
using System;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Common;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 凭证详情
    /// </summary>
    [AutoMap(typeof(VIPVoucher))]
    public  class VIPVoucherDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

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
        /// 条码
        /// </summary>
        public string Barcode { get; set; }

        public virtual ParkSaleTicketClassDto ParkSaleTicketClass { get; set; }
    }
}
