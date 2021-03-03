using Abp.AutoMapper;
using System;
using ThemePark.Common;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleCard.Dto
{    /// <summary>
     /// 凭证详情
     /// </summary>
    [AutoMap(typeof(VIPVoucher))]
    public class ListVipVoucherDto
    {
        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }


        /// <summary>
        /// 基础票类ID
        /// </summary>
        [MapFrom(nameof(VIPVoucher.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClassId))]
        public int TicketClassId { get; set; }


        /// <summary>
        /// 票类名称
        /// </summary>
        [MapFrom(nameof(VIPVoucher.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketClassName))]
        public string TicketClassName { get; set; }

        /// <summary>
        /// 票类人数
        /// </summary>
        [MapFrom(nameof(VIPVoucher.ParkSaleTicketClass), nameof(ParkSaleTicketClass.TicketClass), nameof(TicketClass.TicketType), nameof(TicketType.Persons))]
        public int Persons { get; set; }

        /// <summary>
        /// 促销票类ID
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }

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

    }
}
