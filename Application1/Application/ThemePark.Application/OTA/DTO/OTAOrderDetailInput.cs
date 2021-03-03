using Abp.AutoMapper;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 电商子订单信息
    /// </summary>
    [AutoMapTo(typeof(TOBody))]
    public class OTAOrderDetailInput
    {

        /// <summary>
        /// 公园代码
        /// </summary>
        public short ParkCode { get; set; }

        /// <summary>
        /// 票数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 基础票种编号
        /// </summary>
        public string TicketTypeId { get; set; }

        /// <summary>
        /// 绑定客户
        /// </summary>
        public CustomerInput Customer { get; set; }

        /// <summary>
        /// 实际价格（售价）
        /// </summary>
        public decimal SalePrice { get; set; }
    }
}
