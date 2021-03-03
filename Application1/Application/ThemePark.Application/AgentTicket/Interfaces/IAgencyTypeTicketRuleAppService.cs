using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface IAgencyTypeTicketRuleAppService : IApplicationService
    {
        /// <summary>
        /// 获取代理商订票规则
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        AgencyTypeTicketRuleDto GetAgencyTypeTicketRule(AgencyTypeTicketRuleDto dto);

        /// <summary>
        /// 增加代理商订票规则
        /// </summary>
        /// <param name="dto"></param>
        void AddAgencyTypeTicketRule(AgencyTypeTicketRuleDto dto);

        /// <summary>
        /// 增加代理商订票规则并返回当前实体
        /// </summary>
        /// <param name="dto"></param>
        AgencyTypeTicketRuleDto AddAndReturnAgencyTypeTicketRule(AgencyTypeTicketRuleDto dto);

        /// <summary>
        /// 删除代理商订票规则
        /// </summary>
        /// <param name="dto"></param>
        void DeleteAgencyTypeTicketRuleDto(DeleteInput input);

        /// <summary>
        /// 更新代理商订票规则
        /// </summary>
        /// <param name="dto"></param>
        void UpdateAgencyTypeTicketRuleDto(AgencyTypeTicketRuleDto dto);
    }
}
