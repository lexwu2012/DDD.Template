using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 旅行社预订
    /// </summary>
    public interface ITOHeaderAppService : IApplicationService
    {
        /// <summary>
        /// 确认订单
        /// </summary>
        Task<Result> EditAndConfirmOrderAsync(ConfirmOrderInput input);

        /// <summary>
        /// 搜索订单信息列表
        /// </summary>
        Task<PageResult<TDto>> GetTravelOrdersAsync<TDto>(IPageQuery<TOHeader> query = null);

        /// <summary>
        /// 搜索订单信息
        /// </summary>
        Task<TDto> GetToHeaderAsync<TDto>(IQuery<TOHeader> query);
        
        /// <summary>
        /// 获取TOHeader表最新订单号TOHeaderId
        /// </summary>
        /// <returns></returns>
        Task<string> GetLastToHeaderIdAsync();

        /// <summary>
        /// 搜索订单信息List
        /// </summary>
        Task<List<TDto>> GetToHeadListAsync<TDto>(IQuery<TOHeader> query);

        /// <summary>
        /// 搜索子订单信息List
        /// </summary>
        Task<List<TDto>> GetToBodyListAsync<TDto>(IQuery<TOBody> query);

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="headerId">The header identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> OrderRefundAsync(string headerId);

        /// <summary>
        /// 中心取消确认的旅行社订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> CancelConfirmOrderAsync(string id);

        /// <summary>
        /// 搜索OTA订单信息列表
        /// </summary>
        Task<PageResult<TDto>> GetPagedOtaOrdersAsync<TDto>(OtaSearchOrderDto query = null);

        /// <summary>
        /// 重新发送短信服务
        /// </summary>
        /// <returns></returns>
        Task<Result> ResendMessage(string id);

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> ConfirmOrderAsync(string id);
    }
}
