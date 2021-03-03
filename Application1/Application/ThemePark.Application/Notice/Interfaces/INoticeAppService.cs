using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.Notice.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Notice.Interfaces
{
    /// <summary>
    /// 公告Service
    /// </summary>
    public interface INoticeAppService : IApplicationService
    {
        /// <summary>
        /// 添加公告
        /// </summary>
        Task<Result> AddNoticeAsync(AddNoticeInput input);

        /// <summary>
        /// 修改公告
        /// </summary>
        Task<Result> UpdateNoticeAsync(int id, UpdateNoticeInput input);

        /// <summary>
        /// 浏览次数+1
        /// </summary>
        Task<Result> AddTimes(int id);
        /// <summary>
        /// 删除公告
        /// </summary>
        Task<Result> DeleteNoticeAsync(int id);

        ///// <summary>
        ///// 查询公告
        ///// </summary>
        //Task<PageResult<NoticeDto>> GetNoticeAsync(Query<Core.Notice.Notice> param);

        /// <summary>
        /// 查询公告
        /// </summary>
        Task<PageResult<TDto>> GetNoticeByPageAsync<TDto>(IPageQuery<Core.Notice.Notice> query);

        /// <summary>
        /// 查询旅行社系统的公告
        /// </summary>
        Task<PageResult<TDto>> GetNoticeBySystemTypeIdAsync<TDto>(IPageQuery<Core.Notice.Notice> query);

        /// <summary>
        /// 查询中心票务系统的公告
        /// </summary>
        Task<PageResult<TDto>> GetNoticeBySystemTypeAsync<TDto>(IPageQuery<Core.Notice.Notice> query);

        /// <summary>
        /// 根据条件查询公告
        /// </summary>
        Task<TDto> GetNoticeAsync<TDto>(IQuery<Core.Notice.Notice> query);

        /// <summary>
        /// 根据条件查询公告列表
        /// </summary>
        Task<IList<TDto>> GetNoticeListAsync<TDto>(IQuery<Core.Notice.Notice> query, string defaultSort = "Id Desc", int dataAmount = 10);
    }
}
