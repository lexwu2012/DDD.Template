using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Application.Trade.Dto;
using ThemePark.Core.ParkSale;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapTo(typeof(OtherNonGroupTicket))]
    public class AddOtherParkVendorTicketInput
    {
        /// <summary>
        /// 票所在公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 票卖方公园ID
        /// </summary>
        public int FromParkId { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public int TerminalId { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 客户端添加的售票列表记录
        /// </summary>
        public List<OtherNonGroupTicketInput> TicketInfos { get; set; }

        /// <summary>
        /// 客户端传入的支付信息
        /// </summary>
        public TradeInfoInput TradeInfos { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AddOtherParkVendorTicketInput()
        {
            //交易类型为收入
            TradeInfos = new TradeInfoInput()
            {
                TradeType = TradeType.Income
            };
        }
    }
}
