using System.Collections.Generic;
using Abp.Application.Services;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.BasicTicketType;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace ThemePark.Application.BasicTicketType.Interfaces
{
    /// <summary>
    /// 基础票种应用层服务接口
    /// </summary>
    public interface ITicketClassAppService : IApplicationService
    {
        /// <summary>
        /// 删除基础票类
        /// </summary>
        /// <returns></returns>
        Task<Result> DeleteTicketClassAsync(int id);

        /// <summary>
        /// 增加基础票类
        /// </summary>
        /// <param name="input"></param>
        Task<Result> AddTicketClassAsync(AddTicketClassInput input);

        /// <summary>
        /// 更新票类
        /// </summary>
        Task<Result> UpdateTicketClassAsync(int id,UpdateTicketClassInput input);

        /// <summary>
        /// 查询基础票类
        /// </summary>
        /// <param name="ticketClassPageQuery">查询输入的参数</param>
        /// <returns>查询结果列表</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<TicketClass> ticketClassPageQuery);

        /// <summary>
        /// 根据查询条件返回基础票类
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetTicketClassAsync<TDto>(IQuery<TicketClass> query);

        /// <summary>
        /// 根据查询条件返回基础票类列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetTicketClassListAsync<TDto>(IQuery<TicketClass> query);

        /// <summary>
        /// 根据条件获取基础票类Dropdown列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetTicketClassDropdownAsync(Expression<Func<TicketClass, bool>> exp);
    }
}

