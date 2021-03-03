using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.Trade.Dto
{
    /// <summary>
    /// 交易信息
    /// </summary>
    [AutoMapTo(typeof(TradeInfo))]
    public class TradeInfoInput
    {              
        /// <summary>
        /// 金额
        /// </summary>    
        [Required,Range(0,int.MaxValue)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 支付授权码
        /// </summary>
        /// <remarks>
        /// 通过扫码设备得到用户展示的支付授权码
        /// </remarks>
        [DisplayName("支付授权码")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 收入、支出
        /// </summary>
        public TradeType TradeType { get; set; }

        /// <summary>
        /// 交易详情
        /// </summary>
        public IList<TradeInfoDetailInput> TradeInfoDetails { get; set; }
    }

}
