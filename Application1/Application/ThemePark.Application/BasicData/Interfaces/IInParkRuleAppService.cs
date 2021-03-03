using System.Collections.Generic;
using Abp.Application.Services;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.BasicTicketType;
using System.Threading.Tasks;

namespace ThemePark.Application.BasicData.Interfaces
{
    public interface IInParkRuleAppService : IApplicationService
    {
        /// <summary>
        /// 增加新的入园规则并返回实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddInParkRuleAsync(InParkRuleInput input);

        /// <summary>
        /// 删除存在的入园规则
        /// </summary>
        /// <param name="id"></param>
        Task<Result> DeleteInParkRuleAsync(int id);

        /// <summary>
        /// 更新入园规则
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateInParkRuleAsync(int id, InParkRuleInput input);
    
        /// <summary>
        /// 获取入园规则
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="inParkRulePageQuery">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<InParkRule> inParkRulePageQuery);

        /// <summary>
        /// 根据条件获取入园规则
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetInParkRuleAsync<TDto>(IQuery<InParkRule> query);

        /// <summary>
        /// 根据条件获取入园规则列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetInParkRuleListAsync<TDto>(IQuery<InParkRule> query);

        /// <summary>
        /// 根据条件获取入园规则下拉列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetInParkRuleDropdownAsync();
    }
}

