using System.Linq;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using System.Collections.Generic;
using ThemePark.ApplicationDto;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;
using System;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;

namespace ThemePark.Application.BasicTicketType
{
    /// <summary>
    /// 门市价应用服务层
    /// </summary>
    public class ParkTicketStandardPriceAppService : ThemeParkAppServiceBase, IParkTicketStandardPriceAppService
    {
        #region Fields
        private readonly IRepository<ParkTicketStandardPrice> _standardPriceRepository;
        private readonly IRepository<Park> _parkRepository;
        private readonly IRepository<TicketType> _ticketTypeRepository;
        #endregion

        #region  Ctor
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="standardPriceRepository"></param>
        /// <param name="parkRepository"></param>
        /// <param name="ticketTypeRepository"></param>
        public ParkTicketStandardPriceAppService(IRepository<ParkTicketStandardPrice> standardPriceRepository, IRepository<Park> parkRepository, IRepository<TicketType> ticketTypeRepository)
        {
            _standardPriceRepository = standardPriceRepository;
            _parkRepository = parkRepository;
            _ticketTypeRepository = ticketTypeRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 批量添加公园基础票类门市价
        /// </summary>
        /// <param name="dto"></param>
        public async Task<Result> AddStandardPriceListAsync(List<ParkTicketStandardPriceDto> dto)
        {
            foreach (var group in dto.GroupBy(m => m.TicketTypeId))
            {
                var parkids = group.Select(o => o.ParkId).ToList();

                //检查是否存在重复记录
                var exist = _standardPriceRepository.GetAll().Any(o => parkids.Contains(o.ParkId) && o.TicketTypeId == group.Key);

                if (exist)
                {
                    return Result.FromCode(ResultCode.DuplicateRecord);
                }
            }

            foreach (var item in dto)
            {
                var price = item.MapTo<ParkTicketStandardPrice>();

                await _standardPriceRepository.InsertAsync(price);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 获取公园基础票类门市价
        /// </summary>
        /// <param name="parameter">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public async Task<PageResult<ParkTicketStandardPriceDto>> GetParkTicketStandardPriceAsync(QueryParameter<ParkTicketStandardPrice> parameter)
        {
            var query = _standardPriceRepository.AsNoTrackingAndInclude(m => m.Park, m => m.TicketType);

            var result = await query.ToQueryResultAsync(parameter, ParkTicketStandardPrice => ParkTicketStandardPrice.MapTo<ParkTicketStandardPriceDto>());

            return result;
        }

        /// <summary>
        /// 获取公园基础票类门市价
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询输入参数</param>
        /// <returns>页面查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<ParkTicketStandardPrice> query = null)
        {
            return _standardPriceRepository.AsNoTracking().ToPageResultAsync<ParkTicketStandardPrice, TDto>(query);
        }

        /// <summary>
        /// 删除公园基础票类门市价
        /// </summary>
        /// <param name="id"></param>
        public async Task<Result> DeleteParkTicketStandardPriceAsync(int id)
        {
            await _standardPriceRepository.DeleteAsync(id);

            return Result.Ok();
        }

        /// <summary>
        /// 更新公园基础票类门市价
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="input"></param>
        public async Task<Result> UpdateParkTicketStandardPriceAsync(int Id, ParkTicketStandardPriceUpdateinput input)
        {
            //var exist = _standardPriceRepository.GetAll().Any(i => i.Id != Id && i.ParkId != input.ParkId && i.TicketTypeId == input.TicketTypeId);

            //if (exist)
            //    return Result.FromCode(ResultCode.DuplicateRecord);

            await _standardPriceRepository.UpdateAsync(Id, s => Task.FromResult(input.MapTo(s)));

            return Result.Ok();
        }

        /// <summary>
        /// 根据条件获取公园基础票类门市价
        /// </summary>
        public async Task<TDto> GetParkTicketStandardPriceAsync<TDto>(IQuery<ParkTicketStandardPrice> query)
        {
            return await _standardPriceRepository.GetAll().FirstOrDefaultAsync<ParkTicketStandardPrice, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取公园基础票类门市价列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<Dto>> GetParkTicketStandardPriceListAsync<Dto>(IQuery<ParkTicketStandardPrice> query)
        {
            return await _standardPriceRepository.AsNoTracking().ToListAsync<ParkTicketStandardPrice,Dto>(query);            
        }

        #endregion
    }
}
