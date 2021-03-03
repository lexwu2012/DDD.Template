using Abp.AutoMapper;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapTo(typeof(VIPCard))]
    public  class VipCardInput
    {
        /// <summary>
        /// 票类编号
        /// </summary>    
        public int TicketClassId { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 实际销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

    }
}
