using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.Agencies
{
    /// <summary>
    /// 
    /// </summary>
    public class AgencyTypeAppService : ThemeParkAppServiceBase, IAgencyTypeAppService
    {

        #region Fields

        private readonly IRepository<AgencyType> _agencyTypeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<DefaultPrintSet> _defaultPrintSetRepository;


        #endregion

        #region Ctor
        public AgencyTypeAppService(IRepository<AgencyType> agencyTypeRepository, ICacheManager cacheManager)
        {
            _agencyTypeRepository = agencyTypeRepository;
            _cacheManager = cacheManager;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 增加新的代理商类型 异步
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddAgencyTypeAsync(AddAgencyTypeInput input)
        {
            var check = _agencyTypeRepository.GetAll().Any(t => t.AgencyTypeName == input.AgencyTypeName);
            if (check)
                return Result.FromCode(ResultCode.DuplicateRecord);

            var agencyType = input.MapTo<AgencyType>();
            agencyType.OutTicketType = Core.BasicTicketType.OutTicketType.MulTicket;
            await _agencyTypeRepository.InsertAndGetIdAsync(agencyType);
            return Result.Ok();
        }

        /// <summary>
        /// 异步删除代理商类型
        /// </summary>
        /// <returns></returns>
        public async Task<Result> DeleteAgencyTypeAsync(int id)
        {
            //删除主表
            await _agencyTypeRepository.DeleteAsync(id);
            return Result.Ok();
        }

        /// <summary>
        /// 根据Id获取代理商类型
        /// </summary>
        /// <returns></returns>
        public async Task<AgencyTypeDto> GetAgencyTypeByIdAsync(int id)
        {
            var data = await _agencyTypeRepository.GetAsync(id);
            return data.MapTo<AgencyTypeDto>();
        }

        /// <summary>
        /// 获取代理商类型
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetAgencyTypeAsync<TDto>(IQuery<AgencyType> query)
        {
            return await _agencyTypeRepository.AsNoTracking().FirstOrDefaultAsync<AgencyType, TDto>(query);
        }

        /// <summary>
        /// 根据查询条件返回代理商列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetAgencyTypeListAsync<TDto>(IQuery<AgencyType> query)
        {
            return await _agencyTypeRepository.GetAll().ToListAsync<AgencyType, TDto>(query);
        }


        /// <summary>
        /// 代理商类型列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<AgencyType> query = null)
        {
            return _agencyTypeRepository.AsNoTracking().ToPageResultAsync<AgencyType, TDto>(query);
        }

        /// <summary>
        /// 更新代理商类型
        /// </summary>
        public async Task<Result> UpdateAgencyTypeAsync(int id, UpdateAgencyTypeInput input)
        {
            var check = _agencyTypeRepository.GetAll().Any(p => p.Id != id && p.AgencyTypeName == input.AgencyTypeName);

            if (check)
                return Result.FromCode(ResultCode.DuplicateRecord);

            await _agencyTypeRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));
            ////删除关系
            //await _agencyTypeGroupType.DeleteByAgencyTypeIdAsync(id);
            ////新增关系
            //foreach (var item in input.AgencyTypeGroupTypeInputs)
            //{
            //    await _agencyTypeGroupType.AddAgencyTypeGroupTypeAsync(item);
            //}

            return Result.Ok();
        }

        /// <summary>
        /// 获取出票类型
        /// </summary>
        /// <param name="agencyId"></param>
        /// <returns></returns>
        public async Task<AgencyTypeOutTicketTypeDto> GetAgencyOutTicketTypeAsync(int agencyId)
        {
            var agency = await _agencyTypeRepository.FirstOrDefaultAsync(p => p.Id == agencyId);
            return agency.MapTo<AgencyTypeOutTicketTypeDto>();
        }


        /// <summary>
        /// 更新出票类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAgencyTypeAsync(AgencyTypeOutTicketTypeInput input)
        {
            var agencyType = await _agencyTypeRepository.FirstOrDefaultAsync(p => p.Id == input.Id);

            agencyType.OutTicketType = input.OutTicketType;

            await _agencyTypeRepository.UpdateAsync(agencyType);
            return Result.Ok();
        }

        /// <summary>
        /// 获取所有的代理商类型
        /// </summary>
        /// <returns></returns>
        public IQueryable<AgencyTypeWithDefaultType> GetAgencyTypesAsync()
        {
            return _agencyTypeRepository.AsNoTracking().Select(m => new AgencyTypeWithDefaultType() { Id = m.Id, AgencyTypeName = m.AgencyTypeName, DefaultAgencyType = m.DefaultAgencyType });            
        }

        /// <summary>
        /// 获取所有代理商类型下拉列表
        /// </summary>
        /// <returns></returns>
        public Task<DropdownDto> GetAgencyTypesDropdownAsync()
        {
            return _agencyTypeRepository.AsNoTracking()
                .OrderBy(p => p.Id)
                .ToDropdownDtoAsync(p => new DropdownItem() { Value = p.Id, Text = p.AgencyTypeName });
        }

        /// <summary>
        /// 获取(ota、自有渠道）代理商类型下拉列表
        /// </summary>
        /// <returns></returns>
        public Task<DropdownDto> GetAgencyTypesDropdownByCache2Async()
        {
            return _agencyTypeRepository.AsNoTracking().Where(m => m.DefaultAgencyType == DefaultAgencyType.Ota || m.DefaultAgencyType == DefaultAgencyType.OwnOta)
                .OrderBy(p => p.Id)
                .ToDropdownDtoAsync(p => new DropdownItem() { Value = p.Id, Text = p.AgencyTypeName });
        }

        /// <summary>
        /// 获取所有代理商类型下拉列表
        /// </summary>
        /// <returns></returns>
        public Task<DropdownDto> GetAgencyTypesDropdown4WindowAsync()
        {
            return _agencyTypeRepository.AsNoTracking().Where(m => m.DefaultAgencyType != DefaultAgencyType.Ota && m.DefaultAgencyType != DefaultAgencyType.OwnOta)
                .OrderBy(p => p.Id)
                .ToDropdownDtoAsync(p => new DropdownItem() { Value = p.Id, Text = p.AgencyTypeName });
        }


        #endregion
    }
}
