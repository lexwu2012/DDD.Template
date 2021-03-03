using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.Notice.Dto;
using ThemePark.Core.Notice;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Notice.Interfaces
{
    /// <summary>
    /// 公告类型Service
    /// </summary>
    public interface INoticeTypeAppService : IApplicationService
    {
        /// <summary>
        /// 添加公告类型
        /// </summary>
        Task<Result> AddNoticeTypeAsync(NoticeTypeInput input);
        
        /// <summary>
        /// 编辑公告类型
        /// </summary>
        Task<Result> UpdateNoticeTypeAsync(int id, NoticeTypeInput input);

        /// <summary>
        /// 删除公告类型
        /// </summary>
        Task<Result> DeleteNoticeTypeAsync(int id);

        /// <summary>
        /// 获取公告类型分页列表
        /// </summary>
        Task<PageResult<NoticeTypeDto>> GetNoticeTypeAsync(QueryParameter<NoticeType> parameter);

        /// <summary>
        /// 查询公告类型信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="tickteTypePageQuery">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<NoticeType> tickteTypePageQuery);

        /// <summary>
        /// 根据id获取公告类型
        /// </summary>
        Task<Result<NoticeTypeDto>> GetNoticeTypeById(int id);

        /// <summary>
        /// 获取公告类型
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetNoticeTypeAsync();

        /// <summary>
        /// 根据条件查询公告类型
        /// </summary>
        Task<TDto> GetNoticeAsync<TDto>(IQuery<NoticeType> query);

        /// <summary>
        /// 根据条件查询公告类型列表
        /// </summary>
        Task<IList<TDto>> GetNoticeListAsync<TDto>(IQuery<Core.Notice.NoticeType> query);
    }
}
