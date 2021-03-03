using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Application.DataSync.Interfaces;
using Abp.Auditing;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 数据同步日志服务层
    /// </summary>
    [DisableAuditing]
    public class SyncLogAppService : ThemeParkAppServiceBase, ISyncLogAppService
    {
        #region Fields
        private readonly IRepository<SyncLog, long> _syncLogRepository;
        #endregion Fields

        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="syncLogRepository"></param>
        public SyncLogAppService(IRepository<SyncLog, long> syncLogRepository)
        {
            _syncLogRepository = syncLogRepository;
        }
        #endregion Cotr

        #region Public Methods

        /// <summary>
        /// 添加数据同步日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddSyncLogAsync(SyncLog input)
        {
            await _syncLogRepository.InsertAndGetIdAsync(input);

            return Result.Ok();
        }

        /// <summary>
        /// 根据条件获取数据同步日志
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetSyncLogAsync<TDto>(IQuery<SyncLog> query)
        {
            return await _syncLogRepository.AsNoTracking().FirstOrDefaultAsync<SyncLog, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取数据同步日志列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetSyncLogListAsync<TDto>(IQuery<SyncLog> query)
        {
            return await _syncLogRepository.AsNoTracking().ToListAsync<SyncLog, TDto>(query);
        }

        /// <summary>
        /// 获取同步日志列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<SyncLog> query = null)
        {
            var data = _syncLogRepository.AsNoTracking();

            return data.ToPageResultAsync<SyncLog, TDto>(query);
        }

        #endregion Public Methods
    }
}