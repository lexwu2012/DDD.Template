using Abp.AutoMapper;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡续卡价格配置
    /// </summary>
    [AutoMap(typeof(VipCardRenewalSet))]
    public class VipCardRenewalSetDto
    {
        /// <summary>
        /// 基础票类Id
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 首次续卡价格
        /// </summary>
        public decimal FirstPrice { get; set; }

        /// <summary>
        /// 多次续卡价格
        /// </summary>
        public decimal MulPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual TicketClassDto TicketClass { get; set; }

    }
}
