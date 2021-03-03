using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.AgentTicket
{
    public class AgencyRuleAppService : ThemeParkAppServiceBase, IAgencyRuleAppService
    {
        private IRepository<AgencyRule> _agencyRuleRepository;

        public AgencyRuleAppService(IRepository<AgencyRule> repository)
        {
            _agencyRuleRepository = repository;
        }

        /// <summary>
        /// 增加代理商规则
        /// </summary>
        /// <param name="input"></param>
        public async Task<Result> AddAgencyRuleAsync(AgencyRuleInput input)
        {

            //业务规则验证
            if (!CheckRule(input))
            {
                var result = new Result() { Message = "身份证入园、二维码入园、取票码取票至少选择一项", Code = ResultCode.InvalidData };
                return result;
            }
            var agencyRule = input.MapTo<AgencyRule>();
            await _agencyRuleRepository.InsertAsync(agencyRule);
            return Result.Ok();
        }

        /// <summary>
        /// 增加代理商规则
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAgencyAsync(int id, AgencyRuleInput input)
        {
            //业务规则验证
            if (!CheckRule(input))
            {
                var result = new Result() { Message = "身份证入园、二维码入园、取票码取票至少选择一项", Code = ResultCode.InvalidData };
                return result;
            }

            await _agencyRuleRepository.UpdateAsync(id, o => Task.FromResult(input.MapTo(o)));
            return Result.Ok();
        }

        /// <summary>
        /// 根据代理商规则编号删除
        /// </summary>
        public async Task<Result> DeleteByAgencyRuleIdAsync(int id)
        {
            await _agencyRuleRepository.DeleteAsync(id);
            return Result.Ok();
        }

        /// <summary>
        /// 获取代理商规则
        /// </summary>
        /// <returns></returns>
        public async Task<IList<AgencyRuleOutputDto>> GetAgencyRulesAsync()
        {
            var agencyRules = await _agencyRuleRepository.GetAllListAsync();
            return agencyRules.MapTo<IList<AgencyRuleOutputDto>>();
        }
        /// <summary>
        /// 获取代理商规则
        /// </summary>
        /// <param name="parameter">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public async Task<PageResult<AgencyRuleOutputDto>> GetAgencyRulesAsync(QueryParameter<AgencyRule> parameter)
        {
            var query = _agencyRuleRepository.AsNoTracking();

            var result = await query.ToQueryResultAsync(parameter, agencyRule => agencyRule.MapTo<AgencyRuleOutputDto>());

            return result;
        }
        /// <summary>
        /// 根据代理商规则编号获取代理商规则列表
        /// </summary>
        /// <returns></returns>
        public async Task<AgencyRuleOutputDto> GetByAgencyRuleIdAsync(int id)
        {
            var result = await _agencyRuleRepository.GetAsync(id);
            return result.MapTo<AgencyRuleOutputDto>();
        }

        /// <summary>
        /// 代理商预订规则查询
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<AgencyRule> query = null)
        {
            return _agencyRuleRepository.AsNoTracking().ToPageResultAsync<AgencyRule, TDto>(query);
        }

        /// <summary>
        /// 获取代理商规则下拉列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetAgencyRulesDropdownAsync()
        {
            return _agencyRuleRepository.GetAll()
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.AgencyRuleName, Value = o.Id });
        }

        /// <summary>
        /// 业务规则验证
        /// </summary>
        /// <returns></returns>
        private bool CheckRule(AgencyRuleInput input)
        {
            // 身份证入园、二维码入园、取票码取票三者选其一
            if ((input.CanPidInpark || input.CanQrcodeInpark || input.CanTicketCodeTaken) == false)
                return false;

            return true;
        }
    }
}