using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    /// <summary>
    /// 补票服务接口
    /// </summary>
    public interface IFareAdjustmentAppService : IApplicationService
    {
        /// <summary>
        /// 添加补票记录
        /// </summary>
        /// <returns></returns>
        Task<Result<string>> AddFareAdjustmentAndReturnTradeNumAsync(FareAdjustmentAddNewInput dto, InvoiceInput invoiceInput,int terminalId, int parkId);

        /// <summary>
        /// 作废票
        /// </summary>
        /// <returns></returns>
        Task<Result> CancelTicketAsync(IList<string> fareIds, int terminalId, int parkId);

        /// <summary>
        /// 根据条件获取有效补票记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetFareAdjustmentAsync<TDto>(IQuery<ExcessFare> query);

        /// <summary>
        /// 根据条件获取有效补票记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetFareAdjustmentListAsync<TDto>(IQuery<ExcessFare> query);
    }
}
