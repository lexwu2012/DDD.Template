using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using ThemePark.Application.ReEnter.Dto;
using ThemePark.Application.ReEnter.Interfaces;
using ThemePark.Core.ReEnter;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.ReEnter
{
    /// <summary>
    /// 二次入园规则设置
    /// </summary>
    public class ReEnterEnrollRuleService : ThemeParkAppServiceBase, IReEnterEnrollRuleService
    {

        private readonly IRepository<ReEnterEnrollRule> _reEnterEnrollRuleRepository;
        private readonly IRepository<ReEnterTicketRull> _reEnterTicketRullRepository;

        /// <summary>
        /// 
        /// </summary>
        public ReEnterEnrollRuleService(IRepository<ReEnterEnrollRule> reEnterEnrollRuleRepository
            , IRepository<ReEnterTicketRull> reEnterTicketRullRepository)
        {

            _reEnterEnrollRuleRepository = reEnterEnrollRuleRepository;
            _reEnterTicketRullRepository = reEnterTicketRullRepository;
        }

        /// <summary>
        /// 增加二次入园规则
        /// </summary>
        /// <param name="reEnterEnrollRuleInput"></param>
        /// <param name=""></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> AddReEnterRollRuleAsync(ReEnterEnrollRuleInput reEnterEnrollRuleInput, int terminalId)
        {
            if (reEnterEnrollRuleInput.EnrollStartTime > reEnterEnrollRuleInput.EnrollEndTime)
            {
                return Result.FromError("开始时间不能大于结束时间！！");
            }

            var reEnterEnrollRule = await _reEnterEnrollRuleRepository.FirstOrDefaultAsync(p => p.RuleName == reEnterEnrollRuleInput.RuleName);
            if (reEnterEnrollRule != null)
            {
                return Result.FromError("规则名称已存在！！");
            }

            reEnterEnrollRule = new ReEnterEnrollRule();
            reEnterEnrollRule.EnrollEndTime = reEnterEnrollRuleInput.EnrollEndTime;
            reEnterEnrollRule.EnrollStartTime = reEnterEnrollRuleInput.EnrollStartTime;
            reEnterEnrollRule.LimitedTime = reEnterEnrollRuleInput.LimitedTime;
            reEnterEnrollRule.Remark = reEnterEnrollRuleInput.Remark;
            reEnterEnrollRule.RuleName = reEnterEnrollRuleInput.RuleName;
            reEnterEnrollRule.ValidCount = reEnterEnrollRuleInput.ValidCount;
            reEnterEnrollRule.NeedCheckFp = reEnterEnrollRuleInput.NeedCheckFp;
            reEnterEnrollRule.NeedCheckPhoto = reEnterEnrollRuleInput.NeedCheckPhoto;

            await _reEnterEnrollRuleRepository.InsertAsync(reEnterEnrollRule);

            return Result.Ok();
        }

        /// <summary>
        /// 新增/修改二次入园规则绑定
        /// </summary>
        /// <param name="reEnterTicketRullInputs"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> AddReEnterTicketRullAsync(ReEnterTicketRullInputs reEnterTicketRullInputs, int terminalId)
        {

            foreach(var ticketClassId in reEnterTicketRullInputs.TicketClassIds)
            {
                var reEnterTicketRull = await _reEnterTicketRullRepository.FirstOrDefaultAsync(p => p.TicketClassId == ticketClassId.Id);
                if (reEnterTicketRull != null)
                {
                    reEnterTicketRull.ReEnterEnrollRuleId = reEnterTicketRullInputs.ReEnterEnrollRuleId;
                    await _reEnterTicketRullRepository.UpdateAsync(reEnterTicketRull);
                }
                else
                {
                    reEnterTicketRull = new ReEnterTicketRull();
                    reEnterTicketRull.ReEnterEnrollRuleId = reEnterTicketRullInputs.ReEnterEnrollRuleId;
                    reEnterTicketRull.TicketClassId = ticketClassId.Id;
                    await _reEnterTicketRullRepository.InsertAsync(reEnterTicketRull);
                }
            }
            return Result.Ok();
        }

        /// <summary>
        /// 获取入园规则
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ReEnterEnrollRuleDto>> GetReEnterRulls()
        {
            var query = _reEnterEnrollRuleRepository.AsNoTracking();

            return await query.ProjectTo<ReEnterEnrollRuleDto>().ToListAsync();
        }

        /// <summary>
        /// 获取二次入园规则绑定
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ReEnterTicketRullDto>> GetReEnterTicketRulls()
        {
            var query = _reEnterTicketRullRepository.AsNoTracking();

            return await query.ProjectTo<ReEnterTicketRullDto>().ToListAsync();
        }


        /// <summary>
        /// 修改二次入园规则
        /// </summary>
        /// <param name="reEnterEnrollRuleInput"></param>
        /// <param name=""></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> AlterReEnterRollRuleAsync(ReEnterEnrollRuleInput reEnterEnrollRuleInput, int terminalId)
        {

            if(reEnterEnrollRuleInput.EnrollStartTime> reEnterEnrollRuleInput.EnrollEndTime)
            {
                return Result.FromError("开始时间不能大于结束时间！！");
            }

            var reEnterEnrollRule = await _reEnterEnrollRuleRepository.FirstOrDefaultAsync(p => p.RuleName == reEnterEnrollRuleInput.RuleName && p.Id != reEnterEnrollRuleInput.Id);
            if (reEnterEnrollRule != null)
            {
                return Result.FromError("规则名称已存在！！");
            }
            reEnterEnrollRule = await _reEnterEnrollRuleRepository.FirstOrDefaultAsync(p => p.Id == reEnterEnrollRuleInput.Id);
            reEnterEnrollRule.EnrollEndTime = reEnterEnrollRuleInput.EnrollEndTime;
            reEnterEnrollRule.EnrollStartTime = reEnterEnrollRuleInput.EnrollStartTime;
            reEnterEnrollRule.LimitedTime = reEnterEnrollRuleInput.LimitedTime;
            reEnterEnrollRule.Remark = reEnterEnrollRuleInput.Remark;
            reEnterEnrollRule.RuleName = reEnterEnrollRuleInput.RuleName;
            reEnterEnrollRule.ValidCount = reEnterEnrollRuleInput.ValidCount;
            reEnterEnrollRule.NeedCheckFp = reEnterEnrollRuleInput.NeedCheckFp;
            reEnterEnrollRule.NeedCheckPhoto = reEnterEnrollRuleInput.NeedCheckPhoto;

            await _reEnterEnrollRuleRepository.UpdateAsync(reEnterEnrollRule);

            return Result.Ok();
        }

        /// <summary>
        /// 删除二次入园规则
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> DelReEnterRollRuleAsync(int Id, int terminalId)
        {
            var reEnterEnrollRule = await _reEnterEnrollRuleRepository.FirstOrDefaultAsync(p =>  p.Id == Id);
            await _reEnterEnrollRuleRepository.DeleteAsync(reEnterEnrollRule);

            return Result.Ok();
        }

        /// <summary>
        /// 删除二次入园规则绑定
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> DelReEnterTicketRullAsync(int Id, int terminalId)
        {
            var reEnterTicketRule = await _reEnterTicketRullRepository.FirstOrDefaultAsync(p => p.Id == Id);
            await _reEnterTicketRullRepository.DeleteAsync(reEnterTicketRule);

            return Result.Ok();
        }






    }
}
