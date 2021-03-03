using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt
{

    public interface INonGroupTicketAppService : IApplicationService
    {

        /// <summary>
        /// 新增散客售票记录
        /// </summary>
        /// <returns></returns>
        Task<Result<string>> AddNonGroupTicketAndReturnTradeNumAsync(SaveNonGroupSaleTicketDto dto, int terminalId, int parkId);

        Task<NonGroupTicket> GetTicketById(string id);

        /// <summary>
        /// 查询散客票信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetNonGroupTicketAsync<TDto>(IQuery<NonGroupTicket> query);

        /// <summary>
        /// 根据条码获取散客票信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;NonGroupTicket&gt;.</returns>
        Task<NonGroupTicket> GetNonGroupTicketByBarcodeAsync(string barcode);


        /// <summary>
        /// 根据条码确定门票是否未使用
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CheckTicketUnusedAsync(string barcode);

        /// <summary>
        /// 手动入园
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> ManualInPark(string barcode);

        /// <summary>
        /// 查询散客票信息
        /// </summary>
        /// <returns></returns>
        Task<Result<decimal>> CheckNonGroupTicketAsync(ISaleTicketCheck<NonGroupTicketInput> input);


        /// <summary>
        /// 根据查询条件获取散客票信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetNonGroupTicketListAsync<TDto>(IQuery<NonGroupTicket> query);

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        Task<Result<string>> UpdateNonGroupTicketToInvalidAndReturnOriginalTradeIdAsync(string barCode);

        /// <summary>
        /// 获取散客票默认有效期
        /// </summary>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        Task<Result<int>> GetNonGroupValidAsync();

        /// <summary>
        /// 获取散客有效期
        /// </summary>
        /// <param name="ticketClassId"></param>
        /// <returns></returns>
        Task<int> GetValidDays(int ticketClassId);


    }
}
