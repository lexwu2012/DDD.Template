using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThemePark.Application.Trade.Dto;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.SaleTicekt.Dto
{
    public class AddVendorTicketInput
    {
        /// <summary>
        /// 公园Id
        /// </summary>
        [Required]
        public int ParkId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public int TerminalId { get; set; }

        /// <summary>
        /// 客户端添加的售票列表记录
        /// </summary>
        public List<NonGroupTicketInput> TicketInfos { get; set; }

        /// <summary>
        /// 客户端传入的支付信息
        /// </summary>
        public TradeInfoInput TradeInfos { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AddVendorTicketInput()
        {
            //交易类型为收入
            TradeInfos = new TradeInfoInput()
            {
                TradeType = TradeType.Income
            };
        }
    }
}
