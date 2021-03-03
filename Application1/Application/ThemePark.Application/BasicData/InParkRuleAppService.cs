using System.Collections.Generic;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ThemePark.Application.BasicData
{
    public class InParkRuleAppService : ThemeParkAppServiceBase, IInParkRuleAppService
    {

        #region Fiedls

        private readonly IRepository<InParkRule> _inParkRuleRepository;

        #endregion

        #region Ctor
        public InParkRuleAppService(IRepository<InParkRule> inParkRuleRepository)
        {
            _inParkRuleRepository = inParkRuleRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 增加新的入园规则并返回实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddInParkRuleAsync(InParkRuleInput input)
        {
            DateTime inparkBeginDateTime = Convert.ToDateTime(input.InParkTimeBegin);
            DateTime inparkEndDateTime = Convert.ToDateTime(input.InParkTimeEnd);

            if (input.AppointDate && (input.ValidStartDate==null || input.ValidEndDate==null))
            {
                return Result.FromError("选择指定日期后 必须选择开始日期和结束日期");
            }
            else if (input.AppointDate && input.ValidStartDate.Value> input.ValidEndDate.Value)
            {
                return Result.FromError("开始日期不能大于结束日期");
            }

            if (DateTime.Compare(inparkBeginDateTime, inparkEndDateTime) > 0)
            {
                return Result.FromError("入园开始时间必须小于入园结束时间.");
            }

            if (input.InParkTimesPerDay <= 0 || input.InParkTimesPerPark <= 0)
            {
                return Result.FromError("请填写入园次数限制.");
            }

            var exist = _inParkRuleRepository.GetAll().Any(i => i.InParkRuleName == input.InParkRuleName);

            if (exist)
                return Result.FromCode(ResultCode.DuplicateRecord);

            var inParkRule = input.MapTo<InParkRule>();

            await _inParkRuleRepository.InsertAndGetIdAsync(inParkRule);

            return Result.Ok();
        }

        /// <summary>
        /// 删除存在的入园规则
        /// </summary>
        /// <param name="id"></param>
        public async Task<Result> DeleteInParkRuleAsync(int id)
        {
            await _inParkRuleRepository.DeleteAsync(id);

            return Result.Ok();
        }


        /// <summary>
        /// 更新入园规则
        /// </summary>
        public async Task<Result> UpdateInParkRuleAsync(int id, InParkRuleInput input)
        {
            DateTime inparkBeginDateTime = Convert.ToDateTime(input.InParkTimeBegin);
            DateTime inparkEndDateTime = Convert.ToDateTime(input.InParkTimeEnd);

            if (input.AppointDate && (input.ValidStartDate == null || input.ValidEndDate == null))
            {
                return Result.FromError("选择指定日期后 必须选择开始日期和结束日期");
            }
            else if (input.AppointDate && input.ValidStartDate.Value > input.ValidEndDate.Value)
            {
                return Result.FromError("开始日期不能大于结束日期");
            }

            if (DateTime.Compare(inparkBeginDateTime, inparkEndDateTime) > 0)
            {
                return Result.FromError("入园开始时间必须小于入园结束时间.");
            }

            if (input.InParkTimesPerDay <= 0 || input.InParkTimesPerPark <= 0)
            {
                return Result.FromError("请填写入园次数限制.");
            }

            var check = _inParkRuleRepository.GetAll().Any(i => i.Id != id && i.InParkRuleName == input.InParkRuleName);

            if (check)
                return Result.FromCode(ResultCode.DuplicateRecord);

            await _inParkRuleRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        /// <summary>
        /// 获取入园规则
        /// </summary>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<InParkRule> query = null)
        {
            return _inParkRuleRepository.AsNoTracking().ToPageResultAsync<InParkRule, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取入园规则
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetInParkRuleAsync<TDto>(IQuery<InParkRule> query)
        {
            return await _inParkRuleRepository.AsNoTracking().FirstOrDefaultAsync<InParkRule, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取入园规则列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetInParkRuleListAsync<TDto>(IQuery<InParkRule> query)
        {
            return await _inParkRuleRepository.AsNoTracking().ToListAsync<InParkRule, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取入园规则下拉列表
        /// </summary>
        /// <returns></returns>
        public async Task<DropdownDto> GetInParkRuleDropdownAsync()
        {
            return await _inParkRuleRepository.AsNoTracking()
                .OrderBy(m => m.Id).ToDropdownDtoAsync(m => new DropdownItem { Value = m.Id, Text = m.InParkRuleName });
        }

        #endregion
    }
}
