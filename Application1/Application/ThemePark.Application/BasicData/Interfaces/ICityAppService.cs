using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    /// <summary>
    /// Interface ICityAppService
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService"/>
    public interface ICityAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 获取拥有公园的城市信息
        /// </summary>
        /// <returns></returns>
        Task<IList<CityDto>> GetAllParkCitiesAsync();

        /// <summary>
        /// 获取所有省份信息
        /// </summary>
        /// <returns></returns>
        Task<List<CityDto>> GetAllProvincesAsync();

        /// <summary>
        /// 通过父Id获取地区信息
        /// </summary>
        /// <param name="parentId">父Id</param>
        /// <returns></returns>
        Task<List<CityDto>> GetAreasByParentIdAsync(long parentId);

        /// <summary>
        /// 根据城市Id检索城市
        /// </summary>
        /// <param name="cityIds">城市Id</param>
        /// <returns></returns>
        Task<List<CityDto>> GetCitiesByIdAsync(long[] cityIds);

        /// <summary>
        /// 通过城市名称检索城市
        /// </summary>
        /// <param name="name">城市名称</param>
        /// <returns>以城市名称开头的开头的城市信息</returns>
        Task<List<CityDto>> GetCitiesByNameAsync(string name);

        /// <summary>
        /// 根据id获取城市信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CityDto> GetCityByIdAsync(long id);

        /// <summary>
        /// 通过城市名称检索城市并关联省份信息
        /// </summary>
        /// <param name="cityName">城市名称</param>
        /// <returns>IDictionary&lt;LevelType, City&gt;.</returns>
        Task<IDictionary<LevelType, City>> GetProvinceAndCityByCityNameAsync(string cityName);

        /// <summary>
        /// 通过城市名称获取地区信息（省、市、区、街道）
        /// </summary>
        /// <param name="cityName">城市名称</param>
        /// <returns>Task&lt;ProvincialPositionDictionary&gt;.</returns>
        Task<ProvincialPositionDictionary> GetProvincialPositionByCityAsync(string cityName);

        Task<IList<TDto>> GetProvincesAsync<TDto>(IQuery<City> query);

        #endregion Methods
    }
}