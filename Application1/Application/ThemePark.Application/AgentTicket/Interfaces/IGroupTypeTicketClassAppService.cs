using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.ApplicationDto.AgentTicket;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 团队可售票类型APPService
    /// </summary>
    public interface IGroupTypeTicketClassAppService : IApplicationService
    {
        /// <summary>
        /// 增加新的团队可售票类型
        /// </summary>
        Task<Result> AddGroupTypeTicketClassAsync(List<AddGroupTypeTicketClassInput> input);

        /// <summary>
        /// 更新团队可售票类型信息
        /// </summary>
        Task<Result> UpdateGroupTypeTicketClassAsync(int parkId, int groupTypeId, GroupTypeTicketClassUpdateInput input);

        /// <summary>
        /// 获取团队可售票分页列表
        /// </summary>
        Task<PageResult<GroupTypeTicketClassList>> GetAssembleDataByParkAndGroupTypeId(IPageQuery<GroupTypeTicketClass> query);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="tickteTypePageQuery">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetPagedDataAsync<TDto>(IPageQuery<GroupTypeTicketClass> tickteTypePageQuery);

        /// <summary>
        /// 删除团队可售票类型
        /// </summary>
        Task<Result> DeleteGroupTypeTicketClassAsync(int parkId, int groupTypeId);

        /// <summary>
        /// 获取所有旅游团可售票类
        /// </summary>
        /// <returns></returns>
        IList<GroupTypeTicketClassDto> GetGroupTypeTicketClasses();

        /// <summary>
        /// 根据旅游团类型Id、公园Id获取旅游团可售票类
        /// </summary>
        /// <param name="parkId"></param>
        /// <param name="groupId"></param>
        /// <returns>TicketClassDto类型数据</returns>
        Task<IList<TicketClassDto>> GetTicketClassesByParkAndGroupTypeId(int parkId, int groupId);

        /// <summary>
        /// 根据条件搜索团队可售票类型
        /// </summary>
        /// <typeparam name="TDto">指定的返回类型</typeparam>
        /// <param name="query">搜索条件</param>
        /// <returns>返回第一个搜索到的记录</returns>
        Task<TDto> GetGroupTypeTicketClassAsync<TDto>(IQuery<GroupTypeTicketClass> query);

        /// <summary>
        /// 根据条件获取团体票类列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetGroupTypeTicketClassListAsync<TDto>(IQuery<GroupTypeTicketClass> query);
    }
}
