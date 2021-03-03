using System;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Castle.Core;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.Agencies
{
    /// <summary>
    /// 公园代理商
    /// </summary>
    public class ParkAgencyAppService : ThemeParkAppServiceBase, IParkAgencyAppService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ParkAgency> _parkAgencyRepository;
        private readonly IRepository<ParkArea> _parkAreaRepository;
        private readonly IRepository<Park> _parkRepository;
        private readonly IRepository<Agency> _agencyRepository;
        private readonly IRepository<AgencyUser, long> _agencyUserRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParkAgencyAppService"/> class.
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="parkAgencyRepository">The park agency repository.</param>
        public ParkAgencyAppService(ICacheManager cacheManager, IRepository<ParkAgency> parkAgencyRepository, IRepository<ParkArea> parkAreaRepository, IRepository<Park> parkRepository, IRepository<Agency> agencyRepository, IRepository<AgencyUser, long> agencyUserRepository)
        {
            _cacheManager = cacheManager;
            _parkAgencyRepository = parkAgencyRepository;
            _parkAreaRepository = parkAreaRepository;
            _parkRepository = parkRepository;
            _agencyRepository = agencyRepository;
            _agencyUserRepository = agencyUserRepository;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 新增公园代理商
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddParkAgencyAsync(AddParkAgencyInput input)
        {
            var existed = _parkAgencyRepository.GetAll().Any(p => p.ParkId == input.ParkId && p.AgencyId == input.AgencyId);
            if (existed)
                return Result.FromCode(ResultCode.DuplicateRecord);

            if(input.StartDateTime>input.ExpirationDateTime)
                return Result.FromError("开始时间不能大于结束时间");

            var agencyTypeId = _agencyRepository.GetAll().Where(m => m.Id == input.AgencyId).Select(m => m.AgencyTypeId).First();
            var entity = input.MapTo<ParkAgency>();
            entity.Status = ParkAgencyStatus.Valid;
            entity.AgencyTypeId = agencyTypeId;
            await _parkAgencyRepository.InsertAsync(entity);
            return Result.FromData(input.AgencyId);
        }

        /// <summary>
        /// 删除公园代理商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteParkAgencyAsync(int id)
        {
            await _parkAgencyRepository.DeleteAsync(p => p.Id == id);
            return Result.Ok();
        }

        /// <summary>
        /// 根据公园ID和代理商类型ID获取代理商
        /// </summary>
        /// <param name="parkId">公园ID</param>
        /// <param name="agencyTypeId">代理商类型ID</param>
        /// <returns></returns>
        public async Task<List<DropdownItemCache>> GetAgenciesDropdownItemOnCacheAsync(int parkId, int agencyTypeId)
        {
            return await _parkAgencyRepository.AsNoTracking()
              .Where(o => o.ParkId == parkId && o.AgencyTypeId == agencyTypeId)
              .Select(o => new DropdownItemCache() { Value = o.AgencyId, Text = o.Agency.AgencyName }).ToListAsync();
        }

        /// <summary>
        /// 根据公园ID和代理商类型ID获取代理商下拉列表
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<DropdownDto> GetAgencyDropdownByParkIdAndAgencyTypeId(Expression<Func<ParkAgency, bool>> expression)
        {
            return await _parkAgencyRepository.AsNoTracking()
              .Where(expression)
              .Select(o => new DropdownItem() { Value = o.AgencyId, Text = o.Agency.AgencyName })
              .ToDropdownDtoAsync();
        }

        /// <summary>
        /// 根据公园ID和代理商类型ID获取代理商
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<List<AgencyCombineDto>> GetAgencyByParkIdAndAgencyTypeId(Expression<Func<ParkAgency, bool>> expression)
        {
            return await _parkAgencyRepository.AsNoTracking()
                .Where(expression)
                .Select(o => new AgencyCombineDto
                {
                    AgencyId = o.AgencyId,
                    AgencyName = o.Agency.AgencyName,
                    Checked = false,
                    ProvinceName = o.Agency.Address.Province,
                    CityName = o.Agency.Address.City
                })
                .ToListAsync();
        }

        /// <summary>
        /// 根据公园ID和代理商ID获取代理商类型
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<DropdownDto> GetAgencyTypeDropdownByParkIdAndAgencyId(Expression<Func<ParkAgency, bool>> expression)
        {
            return await _parkAgencyRepository.AsNoTracking()
              .Where(expression)
              .Select(o => new DropdownItem() { Value = o.AgencyTypeId, Text = o.AgencyType.AgencyTypeName })
              .ToDropdownDtoAsync();
        }

        /// <summary>
        /// DataTable获取公园代理商维护列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<ParkAgency> query = null)
        {
            return _parkAgencyRepository.AsNoTracking().ToPageResultAsync<ParkAgency, TDto>(query);
        }

        /// <summary>
        /// 根据ParkId 获取代理商列表 
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetAgenciesDropdownByParkIdAsync(int parkId)
        {

            return _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.ParkId == parkId)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }

        /// <summary>
        /// 根据ParkId 获取公园签约的"OTA用户和自有渠道"代理商列表 
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetParkAgenciesDropdownByParkIdAsync(int parkId)
        {
            //OTA用户和自有渠道用户
            return _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.ParkId == parkId && (o.AgencyType.DefaultAgencyType == DefaultAgencyType.Ota || o.AgencyType.DefaultAgencyType == DefaultAgencyType.OwnOta))
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }

        /// <summary>
        /// 获取公园签约的"OTA用户和自有渠道"代理商列表 
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetParkAgenciesDropdownAsync()
        {
            //OTA用户和自有渠道用户
            return _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.AgencyType.DefaultAgencyType == DefaultAgencyType.Ota || o.AgencyType.DefaultAgencyType == DefaultAgencyType.OwnOta)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }


        /// <summary>
        /// 获取公园签约的"自有渠道"代理商列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetParkAgenciesDropdown4ReportAsync()
        {
            //自有渠道用户
            return _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.AgencyType.DefaultAgencyType == DefaultAgencyType.OwnOta)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }

        /// <summary>
        /// 获取公园签约的"旅行社"代理商列表 
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetParkAgenciesDropdownTravelReportAsync()
        {
            return _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.AgencyType.DefaultAgencyType == DefaultAgencyType.Travel)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }

        /// <summary>
        /// 获取公园签约的"OTA"代理商列表 (获取OTA用户管理的代理商）
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetOtaParkAgenciesDropdownAsync()
        {
            //OTA用户
            return _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.AgencyType.DefaultAgencyType == DefaultAgencyType.Ota)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }

        /// <summary>
        /// 根据代理商ID获取公园列表
        /// </summary>
        public async Task<DropdownDto> GetParksByAgencyIdAsync(int agencyId)
        {
            //TODO：Cuizj use DisableFilter() or change DataFilters.ParkPermission rule for AgencyUser
            //var repository = _parkAgencyRepository as EfRepositoryBase<ThemeParkDbContext, ParkAgency, int>;
            //repository.Context.DisableFilter(DataFilters.ParkPermission);

            return await _parkAgencyRepository.AsNoTracking()
                .Where(o => o.AgencyId == agencyId)
                .OrderBy(o => o.Id)
                .Select(o => new DropdownItem() { Text = o.Park.ParkName, Value = o.ParkId })
                .Distinct()
                .ToDropdownDtoAsync();
        }

        /// <summary>
        /// 获取 代理商类型为旅行社的代理商 的下拉列表(旅行社用户管理用到)
        /// </summary>
        /// <param name="getAll">false 表示排除已经添加了用户的旅行社</param>
        /// <returns></returns>
        public async Task<DropdownDto> GetTravelAgencyDropdownAsync(bool getAll = false)
        {
            var query = _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.AgencyType.DefaultAgencyType == DefaultAgencyType.Travel);

            if (!getAll)
            {
                var agencies = await _agencyUserRepository.GetAll().Select(o => o.AgencyId).Distinct().ToListAsync();
                query = query.Where(o => !agencies.Contains(o.AgencyId));
            }

            return await query.OrderBy(o => o.AgencyId)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }


        /// <summary>
        /// 可根据表达式获取代理商类型为旅行社/OTA的代理商下拉列表
        /// </summary>
        /// <returns></returns>
        public async Task<DropdownDto> GetAgencyDropdownListAsync(Expression<Func<ParkAgency, bool>> expression)
        {
            return await _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(expression)
                .OrderBy(o => o.AgencyId)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }


        /// <summary>
        /// 更新公园代理商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateParkAgencyAsync(int id, UpdateParkAgencyInput input)
        {
            if (input.StartDateTime > input.ExpirationDateTime)
                return Result.FromError("开始时间不能大于结束时间");

            await _parkAgencyRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));
            return Result.Ok();
        }

        /// <summary>
        /// 搜索公园代理商信息
        /// </summary>
        public async Task<TDto> GetParkAgencyAsync<TDto>(IQuery<ParkAgency> query)
        {
            return await _parkAgencyRepository.AsNoTracking().FirstOrDefaultAsync<ParkAgency, TDto>(query);
        }

        /// <summary>
        /// 搜索公园代理商列表信息
        /// </summary>
        public async Task<IList<TDto>> GetParkAgencyListAsync<TDto>(IQuery<ParkAgency> query)
        {
            return await _parkAgencyRepository.AsNoTracking().ToListAsync<ParkAgency, TDto>(query);
        }

        /// <summary>
        /// 根据公园Id获取代理商下拉列表
        /// </summary>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<DropdownDto> GetAgencyDropDownByParkId(int parkId)
        {
            return await _parkAgencyRepository.AsNoTracking()
                .Where(o => o.ParkId == parkId)
                .OrderBy(o => o.Id)
                .Select(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId })
                .ToDropdownDtoAsync();
        }

        /// <summary>
        /// 获取当前用户未配置公园列表
        /// </summary>
        /// <returns></returns>
        public async Task<DropdownDto> GetAgencyParkAsync()
        {
            //已配置的公园id列表
            var usedParkId = await _parkAgencyRepository.AsNoTracking().Select(o => o.ParkId).Distinct().ToListAsync();
            var parkIds = AbpSession.Parks.Where(p => !usedParkId.Contains(p)).ToList();

            return await _parkRepository.AsNoTracking()
                .Where(o => parkIds.Contains(o.Id))
                .OrderBy(o => o.Id)
                .Select(o => new DropdownItem() { Text = o.ParkName, Value = o.Id })
                .ToDropdownDtoAsync();
        }
        #endregion

    }
}