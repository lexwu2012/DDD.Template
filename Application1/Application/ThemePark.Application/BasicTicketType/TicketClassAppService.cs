using System.Collections.Generic;
using System.Linq;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using ThemePark.Common;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicTicketType
{
    /// <summary>
    /// 基础票种应用层服务
    /// </summary>
    public class TicketClassAppService : ThemeParkAppServiceBase, ITicketClassAppService
    {
        #region Fields

        private readonly IRepository<TicketClass> _ticketClassRepository;
        private readonly IRepository<MultiTicketClassPark> _multiTicketClassParkRepository;
        private readonly IRepository<InParkRule> _inParkRuleRepository;
        private readonly IRepository<TicketPrintSet> _ticketPrintSetRepository;

        #endregion

        #region Ctor

        public TicketClassAppService(IRepository<TicketClass> ticketClassRepository, IRepository<MultiTicketClassPark> multiTicketclassParkRepository, IRepository<InParkRule> inParkRuleRepository, IRepository<TicketPrintSet> ticketPrintSetRepository)
        {
            _ticketClassRepository = ticketClassRepository;
            _multiTicketClassParkRepository = multiTicketclassParkRepository;
            _inParkRuleRepository = inParkRuleRepository;
            _ticketPrintSetRepository = ticketPrintSetRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 增加票类
        /// </summary>
        public async Task<Result> AddTicketClassAsync(AddTicketClassInput input)
        {
            if (input.InParkRuleId == 0)
            {
                return Result.FromError("入园规则不能为空，请到入园规则维护页面新增门票类型对应的入园规则.");
            }

            if ((input.TicketClassMode == TicketClassMode.MultiParkTicket || input.TicketClassMode == TicketClassMode.MultiYearCard) && input.InParkIdFilter == null)
                return Result.FromError("多园年卡或多园套票需设置多园设置");

            //根据入园规则计算总入园次数
            var inParkRule = await _inParkRuleRepository.GetAsync(input.InParkRuleId);
            if (inParkRule.TicketClassMode != input.TicketClassMode)
                return Result.FromError("请选择此门票类型对应的入园规则.");

            //基础票类名称、票类编号重复 返回错误
            var existed = _ticketClassRepository.GetAll().Any(p => p.ParkId == input.ParkId && p.TicketTypeId == input.TicketTypeId);
            if (existed)
                return Result.FromCode(ResultCode.DuplicateRecord, "该公园和票种已经配置过票类，请勿重复添加");

            var ticketClass = input.MapTo<TicketClass>();
            ticketClass.InParkIdFilter = input.InParkIdFilter != null ? string.Join(",", input.InParkIdFilter) : null;

            //主表TicketClass插入数据
            await _ticketClassRepository.InsertAndGetIdAsync(ticketClass);

            return Result.Ok();
        }

        /// <summary>
        /// 删除票类 异步
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteTicketClassAsync(int id)
        {
            await _ticketClassRepository.DeleteAsync(id);
            return Result.Ok();
        }


        /// <summary>
        /// 更新票类
        /// </summary>
        public async Task<Result> UpdateTicketClassAsync(int id, UpdateTicketClassInput input)
        {
            if ((input.TicketClassMode == TicketClassMode.MultiParkTicket || input.TicketClassMode == TicketClassMode.MultiYearCard) && input.InParkIdFilterArray == null)
                return Result.FromError("多园年卡或多园套票需设置多园设置");

            //验证入园规则
            var inParkRule = await _inParkRuleRepository.GetAsync(input.InParkRuleId);
            if (inParkRule.TicketClassMode != input.TicketClassMode)
                return Result.FromError("请选择此门票类型对应的入园规则.");

            input.InParkIdFilter = input.InParkIdFilterArray != null ? string.Join(",", input.InParkIdFilterArray) : null;

            //更新主表TicketClass
            await _ticketClassRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        /// <summary>
        /// 基础票类的查询
        /// </summary>
        /// <param name="query">查询输入参数</param>
        /// <returns>查询结果列表</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<TicketClass> query = null)
        {
            return _ticketClassRepository.AsNoTracking().ToPageResultAsync<TicketClass, TDto>(query);
        }

        /// <summary>
        /// 根据查询条件返回基础票类
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetTicketClassAsync<TDto>(IQuery<TicketClass> query)
        {
            return await _ticketClassRepository.GetAll().FirstOrDefaultAsync<TicketClass, TDto>(query);
        }

        /// <summary>
        /// 根据查询条件返回基础票类列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetTicketClassListAsync<TDto>(IQuery<TicketClass> query)
        {
            return await _ticketClassRepository.GetAll().ToListAsync<TicketClass, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取基础票类Dropdown列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetTicketClassDropdownAsync(Expression<Func<TicketClass, bool>> exp)
        {
            var parkIds = AbpSession.Parks;

            exp = exp.And(m => parkIds.Contains(m.ParkId));

            return _ticketClassRepository.GetAll().Where(exp)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.TicketClassName, Value = o.Id });
        }

        #endregion
    }
}
