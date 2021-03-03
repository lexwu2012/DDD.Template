using System.Collections.Generic;
using ThemePark.Application.Trade.Dto;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    ///散客售票保存，包含支付数据和购票记录数据
    /// </summary>
    public class SaveNonGroupSaleTicketDto : ISaleTicketCheck<NonGroupTicketInput>
    {
        /// <summary>
        /// 客户端添加的售票列表记录
        /// </summary>
        public List<NonGroupTicketInput> TicketInfos { get; set; }

        /// <summary>
        /// 客户端传入的支付信息
        /// </summary>
        public TradeInfoInput TradeInfos { get; set; }

        /// <summary>
        /// 发票输入
        /// </summary>
        public InvoiceInput InvoiceInfos { get; set; }

        /// <summary>
        /// 旅行社订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SaveNonGroupSaleTicketDto()
        {
            //交易类型为收入
            TradeInfos = new TradeInfoInput()
            {
                TradeType = TradeType.Income
            };
        }
    }
}
