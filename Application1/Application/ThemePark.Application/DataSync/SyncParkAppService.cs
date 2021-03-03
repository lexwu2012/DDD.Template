using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.DataSync.Dto;
using Abp.AutoMapper;
using System.Linq;
using Abp.Auditing;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// ��԰ͬ����Ϣ�����
    /// </summary>
    public class SyncParkAppService : ThemeParkAppServiceBase, ISyncParkAppService
    {
        #region Fields
        private readonly IRepository<SyncPark> _syncParkRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="syncParkRepository"></param>
        public SyncParkAppService(IRepository<SyncPark> syncParkRepository)
        {
            _syncParkRepository = syncParkRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// ��ӹ�԰ͬ����Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddSyncParkAsync(SyncParkSaveNewInput input)
        {
            var exist = _syncParkRepository.GetAll().Any(o => o.ParkId == input.ParkId);

            if (exist)
            {
                return Result.FromCode(ResultCode.DuplicateRecord);
            }

            var entity = input.MapTo<SyncPark>();

            await _syncParkRepository.InsertAndGetIdAsync(entity);

            return Result.Ok();
        }

        /// <summary>
        /// ɾ����԰ͬ����Ϣ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteSyncParkAsync(int id)
        {
            await _syncParkRepository.DeleteAsync(id);

            return Result.Ok();
        }

        /// <summary>
        /// ���Ĺ�԰ͬ����Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateSyncParkAsync(SyncParkUpdateInput input)
        {
            await _syncParkRepository.UpdateAsync(input.Id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        /// <summary>
        /// ����������ȡ��԰ͬ����Ϣ
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetSyncParkAsync<TDto>(IQuery<SyncPark> query)
        {
            return await _syncParkRepository.AsNoTracking().FirstOrDefaultAsync<SyncPark, TDto>(query);
        }

        /// <summary>
        /// ����������ȡ��԰ͬ����Ϣ�б�
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetSyncParkListAsync<TDto>(IQuery<SyncPark> query)
        {
            return await _syncParkRepository.AsNoTracking().ToListAsync<SyncPark, TDto>(query);
        }

        /// <inheritdoc />
        [DisableAuditing]
        public IList<SyncPark> GetAllList()
        {
            return _syncParkRepository.GetAllList();
        }

        /// <summary>
        /// ��ȡ��԰ͬ����Ϣ�б�
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">ҳ���ѯ�Ĳ���</param>
        /// <returns>ҳ���ѯ���</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<SyncPark> query = null)
        {
            var data = _syncParkRepository.AsNoTracking();

            return data.ToPageResultAsync<SyncPark, TDto>(query);
        }

        #endregion
    }
}