using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Order.Dto
{
    /// <summary>
    /// 子订单详情dto
    /// </summary>
    [AutoMapFrom(typeof(TOBody))]
    public class TOBodyDetailDto
    {
        /// <summary>
        /// cotr
        /// </summary>
        public TOBodyDetailDto()
        {
            TicketDetailDtos = new List<TicketDetailDto>();
        }

        /// <summary>
        /// 子订单号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 主订单编号
        /// </summary> 
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 子订单状态
        /// </summary>
        public OrderState OrderState { get; set; }

        /// <summary>
        /// 子订单状态名称
        /// </summary>
        public string OrderStateName => OrderState.DisplayName();

        /// <summary>
        /// 促销票类编号
        /// </summary> 
        [MapFrom(nameof(TOBody.AgencySaleTicketClass), nameof(AgencySaleTicketClass.ParkSaleTicketClass), nameof(ParkSaleTicketClass.SaleTicketClassName))]
        public string SaleTicketClassName { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(TOBody.Park), nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 入园人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 确认的票数量
        /// </summary>  
        public int Qty { get; set; }

        /// <summary>
        /// 预订数量
        /// </summary>
        public int ReserverQty { get; set; }

        /// <summary>
        /// 实际出票数量（窗口可更改出票数量，但是公园和中心的订单中确认数量不会变）
        /// </summary>
        public int WarrantQty { get; set; }

        /// <summary>
        /// 实际出票金额
        /// </summary>
        public decimal WarrantAmount { get; set; }

        /// <summary>
        /// 子订单中票的退款数量
        /// </summary>
        public int RefundQty { get; set; }

        /// <summary>
        /// 子订单中票的退款总额
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 门市价
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 促销价
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 订单退款信息
        /// </summary>
        public RefundInfo OrderRefundInfo { get; set; }

        /// <summary>
        /// 票详情
        /// </summary>
        public IList<TicketDetailDto> TicketDetailDtos { get; set; }
    }
}
