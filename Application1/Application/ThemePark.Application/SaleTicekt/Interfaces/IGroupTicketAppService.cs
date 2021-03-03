using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    /// <summary>
    /// 团队票类服务接口
    /// </summary>
    public interface IGroupTicketAppService : IApplicationService
    {
        /// <summary>
        /// 更新代理商促销门票
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        Task<Result> UpdateGroupTicketAsync(string id, GroupTicketUpdateForNewInput input);

        Task<GroupTicket> GetTicketById(string id);

        /// <summary>
        /// 根据查询条件获取团体票信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetGroupTicketAsync<TDto>(IQuery<GroupTicket> query);

        /// <summary>
        /// 根据条码获取团体票信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;GroupTicket&gt;.</returns>
        Task<GroupTicket> GetGroupTicketByBarcodeAsync(string barcode);


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
        /// 根据查询条件获取团体票信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetGroupTicketListAsync<TDto>(IQuery<GroupTicket> query);

        /// <summary>
        /// 获取代理商有效期
        /// </summary>
        /// <param name="agencyTypeid"></param>
        /// <param name="agencyId"></param>
        /// <returns></returns>
        Task<int> GetAgencyValidDays(int agencyTypeid, int agencyId);

        /// <summary>
        /// 获取团体票默认有效期
        /// </summary>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        Task<Result<int>> GetGroupValidAsync();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleGroupTicketDto"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result<string>> AddGroupTicketAndReturnTradeNumAsync(SaleGroupTicketDto saleGroupTicketDto,
            int terminalId, int parkId);
    }
}
