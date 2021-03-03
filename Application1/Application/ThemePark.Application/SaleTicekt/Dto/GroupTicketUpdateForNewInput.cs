using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMap(typeof(GroupTicket))]
    public class GroupTicketUpdateForNewInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        [Required]
        [Range(1,int.MaxValue)]
        public int Qty { get; set; }

        /// <summary>
        /// 代理商促销票类Id
        /// </summary>
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal Total { get; set; }
    }
}
