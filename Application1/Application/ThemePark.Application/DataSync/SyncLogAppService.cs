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
    /// ����ͬ����־�����
    /// </summary>
    [DisableAuditing]
    public class SyncLogAppService : ThemeParkAppServiceBase, ISyncLogAppService
    {
        #region Fields
        private readonly IRepository<SyncLog, long> _syncLogRepository;
        #endregion Fields

        #region Cotr
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="syncLogRepository"></param>
        public SyncLogAppService(IRepository<SyncLog, long> syncLogRepository)
        {
            _syncLogRepository = syncLogRepository;
        }
        #endregion Cotr

        #region Public Methods

        /// <summary>
        /// �������ͬ����־
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddSyncLogAsync(SyncLog input)
        {
            await _syncLogRepository.InsertAndGetIdAsync(input);

            return Result.Ok();
        }

        /// <summary>
        /// ����������ȡ����ͬ����־
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetSyncLogAsync<TDto>(IQuery<SyncLog> query)
        {
            return await _syncLogRepository.AsNoTracking().FirstOrDefaultAsync<SyncLog, TDto>(query);
        }

        /// <summary>
        /// ����������ȡ����ͬ����־�б�
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetSyncLogListAsync<TDto>(IQuery<SyncLog> query)
        {
            return await _syncLogRepository.AsNoTracking().ToListAsync<SyncLog, TDto>(query);
        }

        /// <summary>
        /// ��ȡͬ����־�б�
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">ҳ���ѯ�Ĳ���</param>
        /// <returns>ҳ���ѯ���</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<SyncLog> query = null)
        {
            var data = _syncLogRepository.AsNoTracking();

            return data.ToPageResultAsync<SyncLog, TDto>(query);
        }

        #endregion Public Methods
    }
}