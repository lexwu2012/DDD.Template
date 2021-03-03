using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicTicketType.Interfaces
{
    /// <summary>
    /// 促销票类应用服务接口
    /// </summary>
    public interface IParkSaleTicketClassAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 增加公园基础票类门市价
        /// </summary>
        /// <param name="input"></param>
        Task<Result> AddParkSaleTicketClassAsync(ParkSaleTicketClassSaveNewInput input);

        /// <summary>
        /// 删除公园基础票类门市价
        /// </summary>
        /// <param name="id"></param>
        Task<Result> DeleteParkSaleTicketClassAsync(int id);

        /// <summary>
        /// 公园促销票类的查询
        /// </summary>
        /// <param name="parkSaleTicketClassPageQuery">查询输入的参数</param>
        /// <returns>查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<ParkSaleTicketClass> parkSaleTicketClassPageQuery);

        /// <summary>
        /// 根据ID从缓存中获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ParkSaleTicketClassCacheItem> GetOnCacheByIdAsync(int id);

        /// <summary>
        /// 根据条件获取门市价
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetParkSaleTicketClassAsync<TDto>(IQuery<ParkSaleTicketClass> query);

        /// <summary>
        /// 获取票类可入园人数
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        Task<Result<int>> GetTicketClassPersonsAsync(int id);

        /// <summary>
        /// 获取制定票类名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetParkSaleTicketClassNameAsync(int id);

        /// <summary>
        /// 根据条件获取门市价列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetParkSaleTicketClassListAsync<TDto>(IQuery<ParkSaleTicketClass> query);

        /// <summary>
        /// 获取年卡销售类型
        /// </summary>
        /// <param name="parkId"></param>
        /// <param name="ticketClassMode"></param>
        /// <returns></returns>
        Task<IList<ParkSaleTicketClassYearCardDto>> GetSaleCardTypes(int parkId, TicketClassMode ticketClassMode);

        /// <summary>
        /// 根据公园ID获取公园散客票门市价
        /// </summary>
        /// <param name="parkIds">公园ID</param>
        /// <param name="dateTime">公园ID</param>
        /// <returns>公园基础票类门市价DTO</returns>
        Task<IList<TDto>> GetSaleTicketByParkIdsAsync<TDto>(List<int> parkIds, DateTime? dateTime);

        /// <summary>
        /// 根据公园ID获取公园套票票门市价
        /// </summary>
        /// <param name="parkIds">公园ID</param>
        /// <param name="dateTime">公园ID</param>
        /// <returns>公园基础票类门市价DTO</returns>
        Task<IList<TDto>> GetMulSaleTicketByParkIdsAsync<TDto>(List<int> parkIds, DateTime? dateTime);

        /// <summary>
        /// 更新公园基础票类门市价
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateParkSaleTicketClassAsync(int id, ParkSaleTicketClassInput input);

        #endregion Methods
    }
}