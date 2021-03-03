using Abp.AutoMapper;
using Abp.Runtime.Caching;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.EntityFramework.Uow;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.Authorization.Configuration;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 公园信息服务
    /// </summary>
    public class ParkAppService : ThemeParkAppServiceBase, IParkAppService
    {
        #region Fields

        /// <summary>
        /// The _address domain service
        /// </summary>
        private readonly IAddressDomainService _addressDomainService;

        /// <summary>
        /// The _park area repository
        /// </summary>
        private readonly IParkAreaRepository _parkAreaRepository;

        /// <summary>
        /// The _park domain service
        /// </summary>
        private readonly IParkDomainService _parkDomainService;

        /// <summary>
        /// The _park repository
        /// </summary>
        private readonly IParkRepository _parkRepository;

        /// <summary>
        /// The _cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;
        
        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParkAppService"/> class.
        /// </summary>
        /// <param name="parkRepository">The park repository.</param>
        /// <param name="parkDomainService">The park domain service.</param>
        /// <param name="addressDomainService">The address domain service.</param>
        /// <param name="parkAreaRepository">The park area repository.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="themeParkSession">The theme park session.</param>
        public ParkAppService(IParkRepository parkRepository, IParkDomainService parkDomainService, IAddressDomainService addressDomainService,
            IParkAreaRepository parkAreaRepository, ICacheManager cacheManager)
        {
            _parkRepository = parkRepository;
            _parkDomainService = parkDomainService;
            _addressDomainService = addressDomainService;
            _parkAreaRepository = parkAreaRepository;
            _cacheManager = cacheManager;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 增加新的公园
        /// </summary>
        /// <param name="parkInput">The park input.</param>
        /// <returns>Task&lt;Result&lt;ParkDto&gt;&gt;.</returns>
        public async Task<Result<ParkDto>> AddParkAsync(AddParkInput parkInput)
        {
            var park = parkInput.MapTo<Park>();
            var value = await _parkDomainService.CheckDuplicateParkAsync(park);
            if (!string.IsNullOrEmpty(value))
            {
                return Result.FromCode<ParkDto>(ResultCode.DuplicateRecord, value);
            }

            var address = await _addressDomainService.AddAddressAsync(parkInput.AddressInput.ProvinceId, parkInput.AddressInput.CityId,
                parkInput.AddressInput.CountyId, parkInput.AddressInput.StreetId, parkInput.AddressInput.Detail);
            
            await _parkDomainService.AddParkAsync(park, address);

            //给每个公园添加自己的短信模板，在旅行社下单时用到
            //var parkId = await _parkRepository.InsertAndGetIdAsync(park);
            //AddMessageTemplateSettingIfNotExists(parkId,TravelSetting.MessageTemplate, parkInput.MessageTemplateSetting);

            return Result.Ok(park.MapTo<ParkDto>());
        }

        /// <summary>
        /// 根据Id删除公园
        /// </summary>
        /// <param name="parkId">The park identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> DeleteParkAsync(int parkId)
        {
            var park = await _parkRepository.GetAsync(parkId);

            await _addressDomainService.DeleteAddressAsync(park.Address);
            await _parkDomainService.DeleteParkAsync(park);

            return Result.Ok();
        }

        /// <summary>
        /// 查询公园信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<Park> query = null)
        {
            return _parkRepository.AsNoTracking().ToPageResultAsync<Park, TDto>(query);
        }

        /// <summary>
        /// 获取公园Dropdown列表
        /// </summary>
        /// <returns>DropdownDto.</returns>
        public async Task<DropdownDto> GetDropdownAsync()
        {
            return await _parkRepository.AsNoTracking()
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() {Text = o.ParkName, Value = o.Id});
        }

        /// <summary>
        /// 获取没有配置公园区域的公园Dropdown列表
        /// </summary>
        public async Task<DropdownDto> GetNoParkAreaDropdownAsync()
        {
            var parkIds = await _parkAreaRepository.AsNoTracking().Where(o => o.ParkId.HasValue)
                .Select(o => o.ParkId.Value).Distinct().ToListAsync();

            return await _parkRepository.AsNoTracking().Where(o => !parkIds.Contains(o.Id))
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() {Text = o.ParkName, Value = o.Id});
        }

        /// <summary>
        /// 获取当前用户数据权限的公园Dropdown列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetPermissionActiveParksDropdownAsync()
        {
            var parks = AbpSession.Parks;

            return _parkRepository.AsNoTracking().Where(o => parks.Contains(o.Id) && o.IsActive)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() {Text = o.ParkName, Value = o.Id});
        }

        /// <summary>
        /// 获取公园下拉列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<DropdownDto> GetActiveParksDropdownAsync(List<int> ids)
        {
            return _parkRepository.AsNoTracking().Where(o => ids.Contains(o.Id) && o.IsActive)
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.ParkName, Value = o.Id });
        }

        /// <summary>
        /// 获取公园列表
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>IList&lt;ParkDto&gt;.</returns>
        public async Task<List<ParkDto>> GetParksAsync(ParkQueryInput input)
        {
            var query = _parkRepository.AsNoTracking();

            if (input.Cities != null && input.Cities.Any())
            {
                query = query.Where(o => o.Address.CityId.HasValue && input.Cities.Contains(o.Address.CityId.Value));
            }

            if (!string.IsNullOrWhiteSpace(input.ParkName))
            {
                query = query.Where(o => o.ParkName.Contains(input.ParkName));
            }

            query = query.OrderBy(o => o.Id);

            var list = await query.ToListAsync();

            return list.MapTo<List<ParkDto>>();
        }

        /// <summary>
        /// 获取公园列表
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task&lt;PageResult&lt;ParkDto&gt;&gt;.</returns>
        public async Task<PageResult<ParkDto>> GetParksAsync(QueryParameter<Park> parameter)
        {
            var query = _parkRepository.AsNoTrackingAndInclude(park => park.Address);

            var result = await query.ToQueryResultAsync(parameter, park => park.MapTo<ParkDto>());

            return result;
        }

        /// <summary>
        /// 更新公园信息
        /// </summary>
        /// <param name="parkInput">The park input.</param>
        /// <returns>Task&lt;Result&lt;ParkDto&gt;&gt;.</returns>
        public async Task<Result<ParkDto>> UpdateParkAsync(UpdateParkInput parkInput)
        {
            var park = parkInput.MapTo<Park>();
            var value = await _parkDomainService.CheckDuplicateParkAsync(park);
            if (!string.IsNullOrEmpty(value))
            {
                return Result.FromCode<ParkDto>(ResultCode.DuplicateRecord, value);
            }

            park = await _parkRepository.GetAsync(parkInput.Id);
            if (parkInput.AddressInput == null || parkInput.AddressInput.Id != park.AddressId)
            {
                return Result.FromCode<ParkDto>(ResultCode.InvalidData);
            }

            parkInput.MapTo(park);

            //update address
            var address = await _addressDomainService.UpdateAddressAsync(park.Address, parkInput.AddressInput.ProvinceId,
                parkInput.AddressInput.CityId, parkInput.AddressInput.CountyId, parkInput.AddressInput.StreetId,
                parkInput.AddressInput.Detail);

            await _parkDomainService.UpdateParkAsync(park, address);

            return Result.FromData(park.MapTo<ParkDto>());
        }

        /// <summary>
        /// 根据查询条件获取公园
        /// </summary>
        /// <returns></returns>
        public async Task<TDto> GetParkAsync<TDto>(IQuery<Park> query)
        {
            return await _parkRepository.GetAll().FirstOrDefaultAsync<Park, TDto>(query);
        }

        /// <summary>
        /// 根据查询条件获取公园列表
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TDto>> GetParksListAsync<TDto>(IQuery<Park> query)
        {
            return await _parkRepository.GetAll().ToListAsync<Park, TDto>(query);
        }


        /// <summary>
        /// 验证公园parkCode是否有效
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public async Task<bool> ValidateParkCode(short parkCode)
        {
            return await _parkRepository.AsNoTracking().AnyAsync(p => p.ParkCode == parkCode);
        }

     

        /// <inheritdoc />
        public Task<bool> IsParkEnabledAsync(short? parkCode)
        {
            short testParkCode = 23;
            return Task.FromResult(parkCode == testParkCode);
        }

        #endregion Methods

        #region Private Methods

        private void AddMessageTemplateSettingIfNotExists(int parkId,string name, string value)
        {
            var dbcontext = UnitOfWorkManager.Current.GetDbContext<ThemeParkDbContext>();
            if (dbcontext.Settings.Any(s => s.Name == name && s.UserId == null))
            {
                return;
            }

            dbcontext.Settings.Add(new Setting(parkId, null, name, value));
            dbcontext.SaveChanges();
        }

        #endregion
    }
}