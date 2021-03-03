using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    /// 数据同步日志服务层接口
    /// </summary>
    public interface ISyncLogAppService : IApplicationService
    {
        /// <summary>
        /// 添加数据同步日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddSyncLogAsync(SyncLog input);

        /// <summary>
        /// 根据条件获取数据同步日志
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetSyncLogAsync<TDto>(IQuery<SyncLog> query);

        /// <summary>
        /// 根据条件获取数据同步日志列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetSyncLogListAsync<TDto>(IQuery<SyncLog> query);

        /// <summary>
        /// 获取同步日志列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<SyncLog> query = null);
    }
}