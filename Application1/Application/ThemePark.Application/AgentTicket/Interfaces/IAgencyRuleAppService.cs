using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface IAgencyRuleAppService : IApplicationService
    {
        /// <summary>
        /// 增加代理商规则
        /// </summary>
        /// <param name="input"></param>
        Task<Result> AddAgencyRuleAsync(AgencyRuleInput input);

        /// <summary>
        /// 增加代理商规则
        /// </summary>
        /// <param name="input"></param>
        Task<Result> UpdateAgencyAsync(int id, AgencyRuleInput input);

        /// <summary>
        /// 根据代理商规则编号删除
        /// </summary>
        Task<Result> DeleteByAgencyRuleIdAsync(int id);

        /// <summary>
        /// 获取代理商规则
        /// </summary>
        /// <returns></returns>
        Task<IList<AgencyRuleOutputDto>> GetAgencyRulesAsync();
        /// <summary>
        /// 获取代理商规则
        /// </summary>
        /// <returns></returns>
        Task<PageResult<AgencyRuleOutputDto>> GetAgencyRulesAsync(QueryParameter<AgencyRule> parameter);

        /// <summary>
        /// 根据代理商规则编号获取代理商规则列表
        /// </summary>
        /// <returns></returns>
        Task<AgencyRuleOutputDto> GetByAgencyRuleIdAsync(int id);

        /// <summary>
        /// 代理商预订规则查询
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="agencyRulePageQuery">查询输入参数</param>
        /// <returns>查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<AgencyRule> agencyRulePageQuery);

        /// <summary>
        /// 获取代理商规则下拉列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetAgencyRulesDropdownAsync();
    }
}