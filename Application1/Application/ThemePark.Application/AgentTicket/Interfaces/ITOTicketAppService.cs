using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface ITOTicketAppService : IApplicationService
    {
        /// <summary>
        /// 网络取票新增出票记录
        /// </summary>
        /// <param name="inputs">公园ID</param>
        /// <param name="acturalTerminalId">实际出票本公园终端,跨园取票终端为0</param>
        Task<Result<List<string>>> AddWebTicketAsync(ToTicketInput inputs,int acturalTerminalId);

        /// <summary>
        /// 查询网络订单
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetWebTicketOrderDto>>> GetWebTicketOrderAsync(string pidOrTicketCode);

        /// <summary>
        /// 新增旅行社取票记录
        /// </summary>
        /// <param name="input"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result<List<string>>> AddTravelTicketAsync(AddTravelTicketInput input,int terminalId);

        /// <summary>
        /// 根据查询条件获取网络订单信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetTOTicketAsync<TDto>(IQuery<TOTicket> query);

        /// <summary>
        /// 根据条码获取网络订票信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;GroupTicket&gt;.</returns>
        Task<TOTicket> GetTOTicketByBarcodeAsync(string barcode);

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
        /// 根据查询条件获取网络订单信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetTOTicketListAsync<TDto>(IQuery<TOTicket> query);

    }
}
