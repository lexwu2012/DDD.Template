using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;
using System.Linq.Expressions;
using System;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Core.CoreCache.CacheItem;

namespace ThemePark.Application.Agencies.Interfaces
{
    /// <summary>
    /// 公园代理商应用接口
    /// </summary>
    public interface IParkAgencyAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 增加新的代理商类型，异步方法
        /// </summary>
        Task<Result> AddParkAgencyAsync(AddParkAgencyInput input);

        /// <summary>
        /// 根据Id删除代理商类型，异步方法
        /// </summary>
        Task<Result> DeleteParkAgencyAsync(int id);

        /// <summary>
        /// DataTable查询获取代理商类型
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="parkAgencyPageQuery"></param>
        /// <returns></returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<ParkAgency> parkAgencyPageQuery);


        /// <summary>
        /// 获取公园签约的代理商列表(获取OTA用户管理的代理商，所属代理商类型为）
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetParkAgenciesDropdownAsync();

        /// <summary>
        /// 获取代理商下拉列表缓存（根据公园ID和代理商类型ID）
        /// </summary>
        /// <param name="parkId">公园ID</param>
        /// <param name="agencyTypeId">代理商类型ID</param>
        /// <returns></returns>
        Task<List<DropdownItemCache>> GetAgenciesDropdownItemOnCacheAsync(int parkId, int agencyTypeId);

        /// <summary>
        /// 根据公园代理商ID获取公园列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetParksByAgencyIdAsync(int agencyId);

        /// <summary>
        /// 根据ParkId 获取代理商列表 
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetAgenciesDropdownByParkIdAsync(int parkId);

        /// <summary>
        /// 根据ParkId 获取公园签约的"OTA用户和自有渠道"代理商列表 
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetParkAgenciesDropdownByParkIdAsync(int parkId);

        /// <summary>
        /// 获取公园签约的"OTA"代理商列表 
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetOtaParkAgenciesDropdownAsync();

        /// <summary>
        /// 获取公园签约的"自有渠道"代理商列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetParkAgenciesDropdown4ReportAsync();

        /// <summary>
        /// 获取公园签约的"旅行社"代理商列表 
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetParkAgenciesDropdownTravelReportAsync();

        /// <summary>
        /// 获取 代理商类型为旅行社的代理商 的下拉列表(旅行社用户管理用到)
        /// </summary>
        /// <param name="getAll">false 表示排除已经添加了用户的旅行社</param>
        /// <returns></returns>
        Task<DropdownDto> GetTravelAgencyDropdownAsync(bool getAll = false);

        /// <summary>
        /// 搜索公园代理商信息
        /// </summary>
        Task<TDto> GetParkAgencyAsync<TDto>(IQuery<ParkAgency> query);

        /// <summary>
        /// 搜索公园代理商列表信息
        /// </summary>
        Task<IList<TDto>> GetParkAgencyListAsync<TDto>(IQuery<ParkAgency> query);

        /// <summary>
        /// 增加新的代理商类型，异步方法
        /// </summary>
        Task<Result> UpdateParkAgencyAsync(int id, UpdateParkAgencyInput input);

        /// <summary>
        /// 根据公园Id获取代理商
        /// </summary>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyDropDownByParkId(int parkId);

        /// <summary>
        /// 获取当前用户可用公园列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyParkAsync();

        /// <summary>
        /// 根据公园ID和代理商类型ID获取代理商下拉列表
        /// </summary>
        /// <param name="expression">公园ID</param>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyDropdownByParkIdAndAgencyTypeId(Expression<Func<ParkAgency, bool>> expression);

        /// <summary>
        /// 根据公园ID和代理商ID获取代理商类型
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyTypeDropdownByParkIdAndAgencyId(Expression<Func<ParkAgency, bool>> expression);

        /// <summary>
        /// 获取代理商类型为旅行社的代理商下拉列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyDropdownListAsync(Expression<Func<ParkAgency, bool>> expression);

        /// <summary>
        /// 根据公园ID和代理商类型ID获取代理商
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<List<AgencyCombineDto>> GetAgencyByParkIdAndAgencyTypeId(
            Expression<Func<ParkAgency, bool>> expression);

        #endregion Methods
    }
}