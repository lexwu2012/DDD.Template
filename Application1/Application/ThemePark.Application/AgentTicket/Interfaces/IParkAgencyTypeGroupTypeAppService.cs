using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.CoreCache.CacheItem;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 公园代理商类型团体类型应用服务接口
    /// </summary>
    public interface IParkAgencyTypeGroupTypeAppService : IApplicationService
    {
        /// <summary>
        /// 增加代理商类型所属团体类型
        /// </summary>
        /// <param name="input"></param>
        Task<Result> AddAgencyTypeGroupTypeAsync(AddParkAgencyTypeGroupTypeInput input);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> DeleteAsync(int id);

        /// <summary>
        /// 获取带队团体类型下拉列表缓存（根据公园ID和代理商类型ID）
        /// </summary>
        /// <param name="parkId">公园ID</param>
        /// <param name="agencyTypeId">代理商类型ID</param>
        /// <returns></returns>
        Task<List<DropdownItemCache>> GetGroupTypesDropdownItemOnCacheAsync(int parkId, int agencyTypeId);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="parkAgencyTypeGroupTypePageQuery">查询条件</param>
        /// <returns>查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(
            IPageQuery<ParkAgencyTypeGroupType> parkAgencyTypeGroupTypePageQuery);

        /// <summary>
        /// 搜索数据
        /// </summary>
        Task<TDto> GetParkAgencyTypeGroupTypeAsync<TDto>(IQuery<ParkAgencyTypeGroupType> query);

        /// <summary>
        /// 搜索数据
        /// </summary>
        Task<IList<TDto>> GetParkAgencyTypeGroupTypeListAsync<TDto>(IQuery<ParkAgencyTypeGroupType> query);

        /// <summary>
        /// 根据代理商ID搜索数据
        /// </summary>
        Task<IList<TDto>> GetGroupTypeByAgencyIdList<TDto>(int agencyId);

        /// <summary>
        /// 根据条件获取团体类型Dropdown列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetGroupTypeDropdownByAgencyTypeIdAndParkIdAsync(
            Expression<Func<ParkAgencyTypeGroupType, bool>> exp);

        /// <summary>
        /// 根据parkId获取agencyType
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyTypeDropdownByParkIdAsync(Expression<Func<ParkAgencyTypeGroupType, bool>> exp);

        /// <summary>
        /// 查询Id
        /// </summary>
        Task<int> GetIdByParkAgencyTypeGroupTypeAsync(Expression<Func<ParkAgencyTypeGroupType, bool>> expression);

        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateParkAgencyTypeGroupTypeAsync(UpdateParkAgencyTypeGroupTypeInput input);
    }
}
