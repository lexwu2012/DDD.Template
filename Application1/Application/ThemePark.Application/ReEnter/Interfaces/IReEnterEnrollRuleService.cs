using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.ReEnter.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.ReEnter.Interfaces
{
    /// <summary>
    /// 二次入园设置
    /// </summary>
    public interface IReEnterEnrollRuleService : IApplicationService
    {

        /// <summary>
        /// 新增二次入园规则
        /// </summary>
        /// <param name="reEnterEnrollRuleInput"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> AddReEnterRollRuleAsync(ReEnterEnrollRuleInput reEnterEnrollRuleInput, int terminalId);


        /// <summary>
        /// 新增二次入园规则绑定
        /// </summary>
        /// <param name="reEnterTicketRullInputs"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> AddReEnterTicketRullAsync(ReEnterTicketRullInputs reEnterTicketRullInputs, int terminalId);


        /// <summary>
        /// 获取人员规则
        /// </summary>
        /// <returns></returns>
        Task<IList<ReEnterEnrollRuleDto>> GetReEnterRulls();


        /// <summary>
        /// 获取二次入园规则绑定列表
        /// </summary>
        /// <returns></returns>
        Task<IList<ReEnterTicketRullDto>> GetReEnterTicketRulls();
        

        /// <summary>
        /// 修改二次入园规则
        /// </summary>
        /// <param name="reEnterEnrollRuleInput"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> AlterReEnterRollRuleAsync(ReEnterEnrollRuleInput reEnterEnrollRuleInput, int terminalId);

        /// <summary>
        /// 删除二次入园规则
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> DelReEnterRollRuleAsync(int Id, int terminalId);


        /// <summary>
        /// 删除二次入园规则绑定
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> DelReEnterTicketRullAsync(int Id, int terminalId);
    }
}
