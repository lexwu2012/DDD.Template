using Abp.AutoMapper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 城市信息
    /// </summary>
    public class CityAppService : ThemeParkAppServiceBase, ICityAppService
    {
        #region Fields

        /// <summary>
        /// The _city repository
        /// </summary>
        private readonly ICityRepository _cityRepository;

        /// <summary>
        /// The _park repository
        /// </summary>
        private readonly IParkRepository _parkRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CityAppService"/> class.
        /// </summary>
        /// <param name="cityRepository">The city repository.</param>
        /// <param name="parkRepository">The park repository.</param>
        public CityAppService(ICityRepository cityRepository, IParkRepository parkRepository)
        {
            _cityRepository = cityRepository;
            _parkRepository = parkRepository;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 获取拥有公园的城市信息
        /// </summary>
        /// <returns></returns>
        public async Task<IList<CityDto>> GetAllParkCitiesAsync()
        {
            var citiesId = await _parkRepository.GetAll().Where(o => o.Address.CityId.HasValue)
                .Select(o => o.Address.CityId.Value).Distinct().ToListAsync();

            var cities = await _cityRepository.GetCitiesByIdAsync(citiesId.ToArray());

            return cities.MapTo<IList<CityDto>>();
        }

        /// <summary>
        /// 获取所有省份信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<CityDto>> GetAllProvincesAsync()
        {
            var cities = await _cityRepository.GetAllProvincesAsync();
            return cities.MapTo<List<CityDto>>();
        }

        /// <summary>
        /// 通过父Id获取地区信息
        /// </summary>
        /// <param name="parentId">父Id</param>
        /// <returns></returns>
        public async Task<List<CityDto>> GetAreasByParentIdAsync(long parentId)
        {
            var cities = await _cityRepository.GetAreasByParentIdAsync(parentId);
            return cities.MapTo<List<CityDto>>();
        }

        /// <summary>
        /// 根据城市Id检索城市
        /// </summary>
        /// <param name="cityIds">城市Id列表</param>
        /// <returns></returns>
        public async Task<List<CityDto>> GetCitiesByIdAsync(long[] cityIds)
        {
            var cities = await _cityRepository.GetCitiesByIdAsync(cityIds);

            return cities.MapTo<List<CityDto>>();
        }

        /// <summary>
        /// 通过城市名称检索城市
        /// </summary>
        /// <param name="name">城市名称</param>
        /// <returns>以城市名称开头的开头的城市信息</returns>
        public async Task<List<CityDto>> GetCitiesByNameAsync(string name)
        {
            var cities = await _cityRepository.GetCitiesByNameAsync(name);

            return cities.MapTo<List<CityDto>>();
        }

        /// <summary>
        /// 根据id获取城市信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CityDto> GetCityByIdAsync(long id)
        {
            var city = await _cityRepository.GetAsync(id);
            return city.MapTo<CityDto>();
        }

        /// <summary>
        /// 通过城市名称检索城市并关联省份信息
        /// </summary>
        /// <param name="cityName">城市名称</param>
        /// <returns>IDictionary&lt;LevelType, City&gt;.</returns>
        public Task<IDictionary<LevelType, City>> GetProvinceAndCityByCityNameAsync(string cityName)
        {
            return _cityRepository.GetProvinceAndCityByCityNameAsync(cityName);
        }

        /// <summary>
        /// 通过城市名称获取地区信息（省、市、区、街道）
        /// </summary>
        /// <param name="cityName">城市名称</param>
        /// <returns>Task&lt;ProvincialPositionDictionary&gt;.</returns>
        public Task<ProvincialPositionDictionary> GetProvincialPositionByCityAsync(string cityName)
        {
            return _cityRepository.GetProvincialPositionByCityAsync(cityName);
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        public async Task<IList<TDto>> GetProvincesAsync<TDto>(IQuery<City> query)
        {
            return await _cityRepository.GetAll().ToListAsync<City, TDto>(query);
        }

        #endregion Methods
    }
}