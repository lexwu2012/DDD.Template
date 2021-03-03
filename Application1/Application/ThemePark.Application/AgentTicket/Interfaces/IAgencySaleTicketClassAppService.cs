using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 代理商促销票类应用服务接口
    /// </summary>
    public interface IAgencySaleTicketClassAppService : IApplicationService
    {
        /// <summary>
        /// 批处理增加代理商促销门票
        /// </summary>
        /// <param name="inputs"></param>
        Task<Result> AddAgencySaleTicketClassListAsync(List<AgencySaleTicketClassSaveNewInput> inputs);

        /// <summary>
        /// 删除代理商促销门票
        /// </summary>
        /// <param name="id"></param>
        Task<Result> DeleteAgencySaleTicketClassAsync(int id);

        /// <summary>
        /// 更新代理商促销门票
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        Task<Result> UpdateAgencySaleTicketClassAsync(int id, AgencySaleTicketClassUpdateInput input);

        /// <summary>
        /// 根据代理商ID、公园ID、团体类型ID获取团体售票的可售票类
        /// </summary>
        /// <returns>团体售票的可售票类</returns>
        Task<Result<List<AgencySaleTicketClassCacheItem>>> GetAgencySaleTicket4WindowAsync(GetAgencySaleTicketByWindowInput input);

        /// <summary>
        /// 获取票类可入园人数
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        Task<Result<int>> GetTicketClassPersonsAsync(int id);

        /// <summary>
        /// 获取票类名称
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        Task<string> GetAgencySaleTicketClassNameAsync(int id);

        /// <summary>
        /// 获取促销票类信息
        /// </summary>
        Task<TDto> GetAgencySaleTicketClassAsync<TDto>(IQuery<AgencySaleTicketClass> query);

        /// <summary>
        /// 获取促销票类信息
        /// </summary>
        Task<IList<TDto>> GetAgencySaleTicketClassListAsync<TDto>(IQuery<AgencySaleTicketClass> query);

        /// <summary>
        /// 通过ID从缓存中获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AgencySaleTicketClassCacheItem> GetByIdAsync(int id);

        /// <summary>
        /// 根据代理商Id获取团体类型
        /// </summary>
        /// <param name="agencyTypeId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<DropdownDto> GetGroupTypeByAgencyAndParkIdAsync(int agencyTypeId, int parkId);

        /// <summary>
        /// 获取代理商促销票类列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="agencySaleTicketClassPageQuery">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<AgencySaleTicketClass> agencySaleTicketClassPageQuery);

        /// <summary>
        /// 通过agencyId获取代理商可售票类
        /// </summary>
        /// <returns></returns>
        Task<Result<List<ApiAgencySaleTicketTypeDto>>> GetAgencySaleTicketTypeTask(long userId);

        /// <summary>
        /// 根据代理商Id和团体类型Id获取公园列表（旅行社用到）
        /// </summary>
        /// <param name="agencyId"></param>
        /// <param name="groupTypeId"></param>
        /// <returns></returns>
        Task<DropdownDto> GetParksByActiveAgencyIdAndGroupTypeIdAsync(int agencyId, int groupTypeId);

        /// <summary>
        /// 获取代理商促销票类下拉列表
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<DropdownDto> GetAgencySaleTicketClassDropdownAsync(Expression<Func<AgencySaleTicketClass, bool>> exp);
    }
}
