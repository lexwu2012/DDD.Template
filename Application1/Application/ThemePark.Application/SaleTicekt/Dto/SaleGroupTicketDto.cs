using System.Collections.Generic;
using ThemePark.Application.Trade.Dto;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 取团体票传入参数
    /// </summary>
    public class SaleGroupTicketDto : ISaleTicketCheck<GroupTicketInput>
    {
        /// <summary>
        /// cotr
        /// </summary>
        public SaleGroupTicketDto()
        {
            TradeInfos = new TradeInfoInput()
            {
                TradeType = TradeType.Income
            };
        }

        /// <summary>
        /// 票类详情
        /// </summary>
        public List<GroupTicketInput> TicketInfos { get; set; }

        /// <summary>
        /// 支付详情
        /// </summary>
        public TradeInfoInput TradeInfos { get; set; }

        /// <summary>
        /// 输入的发票信息
        /// </summary>
        public InvoiceInput InvoiceInfos { get; set; }

        /// <summary>
        /// 代理商Id（挂账需要关联代理商）
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商类型ID
        /// </summary>
        public int AgencyTypeId { get; set; }

        /// <summary>
        /// 订单号，非空的时候为旅行社订单
        /// </summary>
        public string OrderId { get; set; }
    }
}
