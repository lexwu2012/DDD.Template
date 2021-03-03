using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 预订单应用服务接口
    /// </summary>
    public interface ITOHeaderPreAppService : IApplicationService
    {
        /// <summary>
        /// 根据条件查找预订单
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetTOHeaderPreAsync<TDto>(IQuery<TOHeaderPre> query);

        /// <summary>
        /// 根据条件查找预订单列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<TDto>> GetTOHeadPreListAsync<TDto>(IQuery<TOHeaderPre> query);

        /// <summary>
        /// 搜索旅行社预订单信息列表
        /// </summary>
        Task<PageResult<TDto>> GetPagedTravelPreOrdersAsync<TDto>(TravelSearchOrderDto query = null);

        /// <summary>
        /// 新增预订订单
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&lt;System.String&gt;&gt;.</returns>
        Task<Result<string>> AddOrderPreAsync(AddAgencyReserveInput input);

        /// <summary>
        /// 旅行社站点对预订单进行更改
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateOrderPreAsync(TOHeaderPreEditInput input);

        /// <summary>
        ///  获取分页的预订单列表（有确认的话将获取确认信息）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PageResult<TOHeaderPreDto>> GetTOHeaderPre4CentreOrTravelAsync(TravelSearchOrderDto query = null);

        /// <summary>
        /// 旅行社关闭预订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<Result> CancelTravelOrderAsync(string orderId);

        /// <summary>
        /// 中心和旅行社根据Id获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TOHeaderPreDetailDto> GetTravelOrderDetailByIdAsync(string id);
    }
}
