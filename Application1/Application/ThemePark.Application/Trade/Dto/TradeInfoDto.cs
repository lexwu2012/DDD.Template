using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.Trade.Dto
{
    /// <summary>
    /// 交易记录
    /// </summary>
    [AutoMap(typeof(TradeInfo))]
    public class TradeInfoDto : FullAuditedEntityDto<string>
    {
        /// <summary>
        /// 金额
        /// </summary>   
        public decimal Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 收入、支出
        /// </summary>
        public TradeType TradeType { get; set; }

        /// <summary>
        /// 交易信息详情
        /// </summary>
        public IList<TradeInfoDetailDto> TradeInfoDetails { get; set; }
    }
}
