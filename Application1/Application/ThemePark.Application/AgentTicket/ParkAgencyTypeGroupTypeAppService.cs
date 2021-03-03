using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Core.CoreCache.CacheItem;
using Abp.Runtime.Caching;
using ThemePark.Core;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 公园代理商类型团体类型应用服务
    /// </summary>
    public class ParkAgencyTypeGroupTypeAppService : ThemeParkAppServiceBase, IParkAgencyTypeGroupTypeAppService
    {

        #region Fields

        private readonly IRepository<ParkAgency> _parkAgencyRepository;
        private readonly IRepository<ParkAgencyTypeGroupType> _parkAgencyTypeGroupTypeRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public ParkAgencyTypeGroupTypeAppService(IRepository<ParkAgencyTypeGroupType> parkAgencyTypeGroupTypeRepository,ICacheManager cacheManager,IRepository<ParkAgency> parkAgencyRepository)
        {
            _parkAgencyTypeGroupTypeRepository = parkAgencyTypeGroupTypeRepository;
            _cacheManager = cacheManager;
            _parkAgencyRepository = parkAgencyRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 增加公园代理商类型所属团体类型
        /// </summary>
        public async Task<Result> AddAgencyTypeGroupTypeAsync(AddParkAgencyTypeGroupTypeInput input)
        {
            //公园、代理商和数据库已有数据一样，不让其添加
            var check =
                _parkAgencyTypeGroupTypeRepository.GetAll()
                    .Any(p => p.ParkId == input.ParkId && p.AgencyTypeId == input.AgencyTypeId && p.GroupTypeId == input.GroupTypeId);
            if (check)
                return Result.FromCode(ResultCode.DuplicateRecord);

            var entity = input.MapTo<ParkAgencyTypeGroupType>();
            var result = await _parkAgencyTypeGroupTypeRepository.InsertAndGetIdAsync(entity);
            return Result.Ok();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteAsync(int id)
        {
            await _parkAgencyTypeGroupTypeRepository.DeleteAsync(id);
            return Result.Ok();
        }

        /// <summary>
        /// 根据条件获取团体类型Dropdown列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public async Task<DropdownDto> GetGroupTypeDropdownByAgencyTypeIdAndParkIdAsync(Expression<Func<ParkAgencyTypeGroupType, bool>> exp)
        {
            return await _parkAgencyTypeGroupTypeRepository.GetAll().Where(exp)
                .OrderBy(o => o.Id)
                .Select(o => new DropdownItem() { Text = o.GroupType.TypeName, Value = o.GroupTypeId })
                .Distinct()
                .ToDropdownDtoAsync();
        }

        /// <summary>
        /// 根据公园ID和代理商类型ID获取带队团体类型
        /// </summary>
        /// <param name="parkId">公园ID</param>
        /// <param name="agencyTypeId">代理商类型ID</param>
        /// <returns></returns>
        public async Task<List<DropdownItemCache>> GetGroupTypesDropdownItemOnCacheAsync(int parkId, int agencyTypeId)
        {
            var key = parkId.ToString()+"_"+agencyTypeId.ToString();
            return await _cacheManager.GetGroupTypeDropdownListCache().GetAsync(key,async ()=>
                {
                    var data = await _parkAgencyTypeGroupTypeRepository.AsNoTracking()
                      .OrderBy(o=>o.Id)
                      .Where(o => o.ParkId == parkId && o.AgencyTypeId == agencyTypeId)
                      .Select(o => new DropdownItemCache() { Value = o.GroupTypeId, Text = o.GroupType.TypeName }).ToListAsync();
                    return data;
   
                });
                                       
        }

        /// <summary>
        /// 根据parkId获取agencyType
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<DropdownDto> GetAgencyTypeDropdownByParkIdAsync(Expression<Func<ParkAgencyTypeGroupType, bool>> exp)
        {
            return await _parkAgencyTypeGroupTypeRepository.GetAll().Where(exp)
                .OrderBy(o => o.Id)
                .Select(o => new DropdownItem() { Text = o.AgencyType.AgencyTypeName, Value = o.AgencyTypeId })
                .Distinct()
                .ToDropdownDtoAsync();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(
            IPageQuery<ParkAgencyTypeGroupType> query = null)
        {
            return
                _parkAgencyTypeGroupTypeRepository.AsNoTracking()
                    .ToPageResultAsync<ParkAgencyTypeGroupType, TDto>(query);
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        public async Task<TDto> GetParkAgencyTypeGroupTypeAsync<TDto>(IQuery<ParkAgencyTypeGroupType> query)
        {
            return await _parkAgencyTypeGroupTypeRepository.AsNoTracking().FirstOrDefaultAsync<ParkAgencyTypeGroupType, TDto>(query);
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        public async Task<IList<TDto>> GetParkAgencyTypeGroupTypeListAsync<TDto>(IQuery<ParkAgencyTypeGroupType> query)
        {
            return await _parkAgencyTypeGroupTypeRepository.AsNoTracking().ToListAsync<ParkAgencyTypeGroupType, TDto>(query);
        }

        /// <summary>
        /// 查询Id
        /// </summary>
        public async Task<int> GetIdByParkAgencyTypeGroupTypeAsync(Expression<Func<ParkAgencyTypeGroupType,bool>> expression)
        {
            var entity = await _parkAgencyTypeGroupTypeRepository.AsNoTracking().Where(expression).FirstOrDefaultAsync();
            if (entity != null)
                return entity.Id;
            return 0;
        }

        /// <summary>
        /// 根据代理商ID查询团体类型
        /// </summary>
        public async Task<IList<TDto>> GetGroupTypeByAgencyIdList<TDto>(int agencyId)
        {
            //搜索该代理商所在的代理商类型及公园
            var parkAgencyList = _parkAgencyRepository.AsNoTracking().Where(o => o.AgencyId == agencyId);
            var temp = _parkAgencyTypeGroupTypeRepository.AsNoTracking();
            foreach(var item in parkAgencyList)
            {
                temp=temp.Where(o => o.ParkId == item.ParkId && o.AgencyTypeId == item.AgencyTypeId);
            }
            
            var list=await temp.Select(o => o.GroupType).Distinct().ToListAsync();
            return list.MapTo<IList<TDto>>();
        }

        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateParkAgencyTypeGroupTypeAsync(UpdateParkAgencyTypeGroupTypeInput input)
        {
            await _parkAgencyTypeGroupTypeRepository.UpdateAsync(input.Id,m => Task.FromResult(input.MapTo(m)));
            return Result.Ok();
        }

        #endregion

    }
}
