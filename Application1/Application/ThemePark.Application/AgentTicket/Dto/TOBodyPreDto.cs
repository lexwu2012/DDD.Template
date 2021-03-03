using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 预订子订单模型
    /// </summary>
    [AutoMap(typeof(TOBodyPre))]
    public class TOBodyPreDto
    {
        /// <summary>
        /// 子订单主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 主订单编号
        /// </summary> 
        public string TOHeaderPreId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>  
        public int Seq { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>    
        public int ParkId { get; set; }

        /// <summary>
        /// 预订公园名称
        /// </summary>
        [MapFrom(nameof(TOBodyPre.Park), nameof(Core.BasicData.Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>
        [MapFrom(nameof(TOBodyPre.AgencySaleTicketClass), nameof(AgencySaleTicketClass.AgencySaleTicketClassName))]
        public string SaleTicketClassName { get; set; }

        /// <summary>
        /// 子订单状态
        /// </summary>
        public OrderState OrderState { get; set; }

        /// <summary>
        /// 子订单状态名称
        /// </summary>
        public string StateName => OrderState.DisplayName();

        /// <summary>
        /// 预订票数量
        /// </summary>  
        public int Qty { get; set; }

        /// <summary>
        /// 确认票数
        /// </summary>
        public int? ConfirmQty { get; set; }

        /// <summary>
        /// 商品单价
        /// </summary>    
        public decimal Price { get; set; }

        /// <summary>
        /// 实际价格
        /// </summary>    
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        public decimal ParkSettlementPrice { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 订票人电话
        /// </summary>
        [MapFrom(nameof(TOBodyPre.Customer), nameof(Customer.PhoneNumber))]
        public string Phone { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        [MapFrom(nameof(TOBodyPre.Customer), nameof(Customer.Pid))]
        public string Pid { get; set; }

        /// <summary>
        /// 订票人姓名
        /// </summary>
        [MapFrom(nameof(TOBodyPre.Customer), nameof(Customer.CustomerName))]
        public string CustomName { get; set; }
    }
}
