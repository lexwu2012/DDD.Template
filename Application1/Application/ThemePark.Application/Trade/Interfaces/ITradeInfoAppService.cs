using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.Trade.Dto;
using ThemePark.Core.TradeInfos;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Trade.Interfaces
{
    /// <summary>
    /// 交易记录AplicationService
    /// </summary>
    public interface ITradeInfoAppService : IApplicationService
    {
        /// <summary>
        /// 新增交易记录
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <param name="agencyId"></param>
        /// <returns></returns>
        Task<Result<string>> AddTradeInfoAndReturnTradeInfoIdAsyn(TradeInfoInput input, int parkId, int? agencyId);

        /// <summary>
        /// 获取特定时间段的销售总额(收入-支出)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        decimal GetSalesAmountByTime(DateTime timeFrom, DateTime timeTo);
    }
}
