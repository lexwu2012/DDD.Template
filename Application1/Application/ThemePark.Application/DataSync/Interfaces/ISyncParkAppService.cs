using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;
using ThemePark.Application.DataSync.Dto;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    /// 公园同步记录服务接口
    /// </summary>
    public interface ISyncParkAppService : IApplicationService
    {
        /// <summary>
        /// 增加新的数据同步乐园数据并返回实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddSyncParkAsync(SyncParkSaveNewInput input);

        /// <summary>
        /// 删除存在的数据同步乐园数据
        /// </summary>
        /// <param name="id"></param>
        Task<Result> DeleteSyncParkAsync(int id);

        /// <summary>
        /// 更新数据同步乐园数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateSyncParkAsync(SyncParkUpdateInput input);

        /// <summary>
        /// 根据条件获取数据同步乐园数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetSyncParkAsync<TDto>(IQuery<SyncPark> query);

        /// <summary>
        /// 根据条件获取数据同步乐园数据列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetSyncParkListAsync<TDto>(IQuery<SyncPark> query);

        /// <summary>
        /// 取所有的同步乐园公园
        /// </summary>
        /// <returns></returns>
        IList<SyncPark> GetAllList();

        /// <summary>
        /// 获取公园同步信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<SyncPark> query = null);
    }
}
