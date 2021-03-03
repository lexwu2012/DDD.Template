using System.Collections.Generic;
using Abp.Application.Services;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.BasicTicketType;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Interfaces
{
    /// <summary>
    /// 基础票种应用服务接口
    /// </summary>
    public interface ITicketTypeAppService : IApplicationService
    {
        /// <summary>
        /// 添加票类型
        /// </summary>
        /// <param name="input"></param>
        Task<Result> AddTicketTypeAsync(TicketTypeAddNewInput input);

        /// <summary>
        /// 更新票类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateTicketTypeAsync(string id, TicketTypeAddNewInput input);

        /// <summary>
        /// 更新票序号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateTicketTypeSortAsync(string id, TicketTypeSortInput input);

        /// <summary>
        /// 删除票类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> DeleteTicketTypeAsync(string id);

        /// <summary>
        /// 获取票种类型
        /// </summary>
        /// <param name="parameter">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TicketTypeDto>> GetTicketTypeAsync(QueryParameter<TicketType> parameter);

        /// <summary>
        /// 查询票种信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="tickteTypePageQuery">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<TicketType> tickteTypePageQuery);

        /// <summary>
        /// 根据条件获取基础票种
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetTicketTypeAsync<TDto>(IQuery<TicketType> query);

        /// <summary>
        /// 根据条件获取基础票种列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetTicketTypeListAsync<TDto>(IQuery<TicketType> query);

        /// <summary>
        /// 获取基础票类下拉列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        DropdownDto<string> GetTicketTypesDropdownAsync();
    }
}

