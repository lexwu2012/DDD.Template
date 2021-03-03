using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    /// <summary>
    /// 公园分区，实现数据权限
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService"/>
    public interface IParkAreaAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 新增公园分区
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Result.</returns>
        Task<Result<ParkArea>> AddParkAreaAsync(AddParkAreaInput input);

        /// <summary>
        /// 获取分区所关联的公园
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetParkAreaOwnParksAsync(int id);

        /// <summary>
        /// 获取所有公园分区，树形结构
        /// </summary>
        Task<List<ParkAreaTreeDto>> GetParkAreaTreeAsync();

        /// <summary>
        /// 更新公园分区树形结构
        /// </summary>
        /// <param name="input">确保各节点DLR顺序，或者是层级顺序</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateParkAreaTreeAsync(UpdateParkAreaTreeInput input);

        /// <summary>
        /// 根据条件获取公园分区
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetParkAreaAsync<TDto>(IQuery<ParkArea> query);

        /// <summary>
        /// 根据条件获取公园分区
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<TDto>> GetParkAreasAsync<TDto>(IQuery<ParkArea> query);

        #endregion Methods
    }
}