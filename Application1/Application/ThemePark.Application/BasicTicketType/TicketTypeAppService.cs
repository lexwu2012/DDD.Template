using System.Collections.Generic;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Core.BasicTicketType;
using System.Linq;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.BasicTicketType;

namespace ThemePark.Application.BasicTicketType
{
    /// <summary>
    /// 基础票种应用服务
    /// </summary>
    public class TicketTypeAppService : ThemeParkAppServiceBase, ITicketTypeAppService
    {
        #region Fields

        private readonly IRepository<TicketType, string> _ticketTypeRepository;

        #endregion

        #region Ctor
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ticketTypeRepository"></param>
        public TicketTypeAppService(IRepository<TicketType, string> ticketTypeRepository)
        {
            _ticketTypeRepository = ticketTypeRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 添加票类型
        /// </summary>
        /// <param name="input"></param>
        public async Task<Result> AddTicketTypeAsync(TicketTypeAddNewInput input)
        {
            if (input.IsLimited == true && input.Remark == null)
            {
                return Result.FromError("请填写入园条件");
            }

            //存在没删除的重复基础票种
            var existed = _ticketTypeRepository.GetAll().Any(m => m.Id == input.Id);

            if (existed)
                return Result.FromCode(ResultCode.DuplicateRecord, "票种编号重复存在");

            var ticketType = input.MapTo<TicketType>();
            await _ticketTypeRepository.InsertAndGetIdAsync(ticketType);

            return Result.Ok();

            ////去除过滤器用来查询数据是否有重复的主键，有重复的主键会发生重复住键错误
            //using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            //{
            //    var entity = _ticketTypeRepository.GetAll().FirstOrDefault(m => m.Id == input.Id && m.IsDeleted);

            //    if (entity != null)
            //    {
            //        return Result.FromCode<TicketTypeDto>(ResultCode.DuplicateRecord, "票种编号重复存在");
            //        ////存在之前删除的记录则更新
            //        //input.IsDeleted = false;
            //        //await _ticketTypeRepository.UpdateAsync(entity.Id, p => Task.FromResult(input.MapTo(p)));
            //    }
            //    else
            //    {
            //        var ticketType = input.MapTo<TicketType>();
            //        await _ticketTypeRepository.InsertAndGetIdAsync(ticketType);
            //    }

            //    return Result.Ok();
            //}

        }

        /// <summary>
        /// 更新票类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateTicketTypeAsync(string id, TicketTypeAddNewInput input)
        {
            if (input.IsLimited == true && input.Remark == null)
            {
                return Result.FromError("请填写入园条件");
            }

            await _ticketTypeRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        /// <summary>
        /// 更新票序号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateTicketTypeSortAsync(string id, TicketTypeSortInput input)
        {
            await _ticketTypeRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        /// <summary>
        /// 删除票类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteTicketTypeAsync(string id)
        {
            await _ticketTypeRepository.DeleteAsync(id);

            return Result.Ok();
        }

        /// <summary>
        /// 获取票种类型
        /// </summary>
        /// <param name="parameter">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public async Task<PageResult<TicketTypeDto>> GetTicketTypeAsync(QueryParameter<TicketType> parameter)
        {
            var query = _ticketTypeRepository.AsNoTracking();

            var result = await query.ToQueryResultAsync(parameter, ticketType => ticketType.MapTo<TicketTypeDto>());

            return result;
        }

        /// <summary>
        /// 查询获取列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">查询条件</param>
        /// <returns>列表结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<TicketType> query = null)
        {
            return _ticketTypeRepository.AsNoTracking().ToPageResultAsync<TicketType, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取基础票种
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetTicketTypeAsync<TDto>(IQuery<TicketType> query)
        {
            return await _ticketTypeRepository.AsNoTracking().FirstOrDefaultAsync<TicketType, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取基础票种列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetTicketTypeListAsync<TDto>(IQuery<TicketType> query)
        {
            return await _ticketTypeRepository.AsNoTracking().ToListAsync<TicketType, TDto>(query);
        }

        /// <summary>
        /// 获取基础票类下拉列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public DropdownDto<string> GetTicketTypesDropdownAsync()
        {
            var ticketTypes = _ticketTypeRepository.GetAll()
                .OrderBy(o => o.Id);
            return new DropdownDto<string>(ticketTypes.Select(o => new DropdownItem<string>() { Text = o.TicketTypeName, Value = o.Id }));
        }
        #endregion
    }
}
