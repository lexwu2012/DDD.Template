using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 团体购票详情
    /// </summary>
    [AutoMap(typeof(GroupTicket))]
    public class GroupTicketInput:ITicketInfo
    {
        /// <summary>
        /// 子订单Id
        /// </summary>
        public string TOBodyId { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>    
        [Range(1,int.MaxValue)]
        public int AgencyId { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 门票数量
        /// </summary> 
        [Range(1,int.MaxValue)]   
        public int Qty { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>    
        [Range(1,int.MaxValue)]
        public int AgencySaleTicketClassId { get; set; }


        /// <summary>
        /// 实际价格
        /// </summary>    
        [Required]
        public decimal SalePrice { get; set; }


        /// <summary>
        /// 总票输出、分票输出
        /// </summary>
        public int IsAllOutPut { get; set; }

        /// <summary>
        /// 票类型，继承自ITicketInfo接口
        /// </summary>
        public int TicketClassId => AgencySaleTicketClassId;

        /// <summary>
        /// 
        /// </summary>
        public TicketClassMode TicketClassMode { get; set; }
    }
}
