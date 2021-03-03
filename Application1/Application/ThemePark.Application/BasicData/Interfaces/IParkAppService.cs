using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    /// <summary>
    /// Interface IParkAppService
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService"/>
    public interface IParkAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 增加新的公园
        /// </summary>
        /// <param name="parkInput">The park input.</param>
        /// <returns>Task&lt;Result&lt;ParkDto&gt;&gt;.</returns>
        Task<Result<ParkDto>> AddParkAsync(AddParkInput parkInput);

        /// <summary>
        /// 根据Id删除公园
        /// </summary>
        /// <param name="parkId">The park identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> DeleteParkAsync(int parkId);

        /// <summary>
        /// 查询公园信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="parkPageQuery"></param>
        /// <returns></returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<Park> parkPageQuery);

        /// <summary>
        /// 获取公园Dropdown列表
        /// </summary>
        /// <returns>DropdownDto.</returns>
        Task<DropdownDto> GetDropdownAsync();

        /// <summary>
        /// 获取没有配置公园区域的公园Dropdown列表
        /// </summary>
        Task<DropdownDto> GetNoParkAreaDropdownAsync();
        
        /// <summary>
        /// 获取当前用户数据权限的公园Dropdown列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetPermissionActiveParksDropdownAsync();

        /// <summary>
        /// 获取公园列表
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>IList&lt;ParkDto&gt;.</returns>
        Task<List<ParkDto>> GetParksAsync(ParkQueryInput input);

        /// <summary>
        /// 获取公园列表
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task&lt;PageResult&lt;ParkDto&gt;&gt;.</returns>
        Task<PageResult<ParkDto>> GetParksAsync(QueryParameter<Park> parameter);

        /// <summary>
        /// 更新公园信息
        /// </summary>
        /// <param name="parkInput">The park input.</param>
        /// <returns>Task&lt;Result&lt;ParkDto&gt;&gt;.</returns>
        Task<Result<ParkDto>> UpdateParkAsync(UpdateParkInput parkInput);

        /// <summary>
        /// 根据查询条件获取公园
        /// </summary>
        /// <returns></returns>
        Task<TDto> GetParkAsync<TDto>(IQuery<Park> query);

        /// <summary>
        /// 根据查询条件获取公园列表
        /// </summary>
        /// <returns></returns>
        Task<IList<TDto>> GetParksListAsync<TDto>(IQuery<Park> query);

        /// <summary>
        /// 验证公园
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        Task<bool> ValidateParkCode(short parkCode);

        /// <summary>
        /// 判断指定公园是否在系统中启用
        /// </summary>
        Task<bool> IsParkEnabledAsync(short? parkCode);

        /// <summary>
        /// 获取公园下拉列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<DropdownDto> GetActiveParksDropdownAsync(List<int> ids);

        #endregion Methods
    }
}