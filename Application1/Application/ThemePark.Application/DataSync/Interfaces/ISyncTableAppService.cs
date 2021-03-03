using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    ///  数据表同步记录应用服务接口
    /// </summary>
    public interface ISyncTableAppService : IApplicationService
    {
        /// <summary>
        /// 新增数据表同步记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddSyncTableAsync(SyncTableSaveNewInput input);

        /// <summary>
        /// 删除数据表同步记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> DeleteSyncTableAsync(int id);

        /// <summary>
        /// 更新数据表同步记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateSyncTableAsync(SyncTableUpdateInput input);

        /// <summary>
        /// 根据条件获取数据表同步记录
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        Task<TDto> GetSyncTableAsync<TDto>(IQuery<SyncTable> query);

        /// <summary>
        /// 根据条件获取数据表同步记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<SyncTable>> GetSyncTableListAsync(IQuery<SyncTable> query);

        /// <summary>
        /// 获取数据同步信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<SyncTable> query = null);

        /// <summary>
        /// 获取表名称列表
        /// </summary>
        /// <returns>DropdownDto&lt;System.String&gt;.</returns>
        DropdownDto<string> GetTableDropdownList();
    }
}