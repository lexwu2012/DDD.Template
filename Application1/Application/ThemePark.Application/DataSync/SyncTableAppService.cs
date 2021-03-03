using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.DataSync.Dto;
using Abp.AutoMapper;
using System.Linq;
using Abp.Dependency;
using ThemePark.EntityFramework;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 数据表同步记录服务层
    /// </summary>
    public class SyncTableAppService : ThemeParkAppServiceBase, ISyncTableAppService
    {
        #region Fields
        private readonly IRepository<SyncTable> _syncTableRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="syncTableRepository"></param>
        public SyncTableAppService(IRepository<SyncTable> syncTableRepository)
        {
            _syncTableRepository = syncTableRepository;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// 新增数据表同步记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddSyncTableAsync(SyncTableSaveNewInput input)
        {
            var exist = _syncTableRepository.GetAll().Any(o => o.EntityName == input.EntityName);

            if (exist)
            {
                return Result.FromCode(ResultCode.DuplicateRecord, "同一张表不允许同时存在上传和下载动作");
            }
            
            var entity = input.MapTo<SyncTable>();

            await _syncTableRepository.InsertAndGetIdAsync(entity);

            return Result.Ok();
        }

        /// <summary>
        /// 删除数据表同步记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteSyncTableAsync(int id)
        {
            await _syncTableRepository.DeleteAsync(id);

            return Result.Ok();
        }

        /// <summary>
        /// 更新数据表同步记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateSyncTableAsync(SyncTableUpdateInput input)
        {
            await _syncTableRepository.UpdateAsync(input.Id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        /// <summary>
        /// 根据条件获取数据表同步记录
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public async Task<TDto> GetSyncTableAsync<TDto>(IQuery<SyncTable> query)
        {
            return await _syncTableRepository.AsNoTracking().FirstOrDefaultAsync<SyncTable, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取数据表同步记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<SyncTable>> GetSyncTableListAsync(IQuery<SyncTable> query)
        {
            return await _syncTableRepository.AsNoTracking().Where(query).ToListAsync();
        }

        /// <summary>
        /// 获取数据同步信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<SyncTable> query = null)
        {
            var data = _syncTableRepository.AsNoTracking();

            return data.ToPageResultAsync<SyncTable, TDto>(query);
        }

        /// <summary>
        /// 获取表名称列表
        /// </summary>
        /// <returns>DropdownDto&lt;System.String&gt;.</returns>
        public DropdownDto<string> GetTableDropdownList()
        {
            var dbContext = IocManager.Instance.Resolve<ThemeParkDbContext>();
            var medata = ((IObjectContextAdapter)dbContext).ObjectContext.MetadataWorkspace;
            var tables = medata.GetItemCollection(DataSpace.SSpace)
                .GetItems<EntityContainer>().Single().BaseEntitySets.OfType<EntitySet>()
                .Where(o => !o.MetadataProperties.Contains("Type") || o.MetadataProperties["Type"].ToString() == "Tables");

            var itemList = new DropdownDto<string>(tables.OrderBy(o => o.Table).Select(o => new DropdownItem<string>() { Text = o.Table, Value = o.Table }));

            return itemList;
        }

        #endregion
    }
}