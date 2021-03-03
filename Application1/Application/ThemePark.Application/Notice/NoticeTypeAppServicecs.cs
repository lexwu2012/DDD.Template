using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.Notice.Dto;
using ThemePark.Application.Notice.Interfaces;
using ThemePark.Core.Notice;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.Notice
{
    /// <summary>
    /// 公告类型Service
    /// </summary>
    public class NoticeTypeAppService: ThemeParkAppServiceBase, INoticeTypeAppService
    {

        #region Fields

        private readonly IRepository<NoticeType> _noticeTypeRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造
        /// </summary>
        public NoticeTypeAppService(IRepository<NoticeType> noticeTypeRepository)
        {
            _noticeTypeRepository = noticeTypeRepository;
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// 添加公告类型
        /// </summary>
        public async Task<Result> AddNoticeTypeAsync(NoticeTypeInput input)
        {
            var check = _noticeTypeRepository.GetAll().Any(o => o.Name == input.Name);

            if (check)
                return Result.FromCode<NoticeTypeDto>(ResultCode.DuplicateRecord);

            var noticeType = input.MapTo<NoticeType>();
            await _noticeTypeRepository.InsertAndGetIdAsync(noticeType);

            return Result.Ok(noticeType.MapTo<NoticeTypeDto>());
        }

        /// <summary>
        /// 编辑公告类型
        /// </summary>
        public async Task<Result> UpdateNoticeTypeAsync(int id,NoticeTypeInput input)
        {
            var check = _noticeTypeRepository.GetAll().Any(i => i.Id == id);
            //查看是否存在该数据
            if (!check)
                return Result.FromCode(ResultCode.NoRecord);
            var checkDuplicate = _noticeTypeRepository.GetAll().Any(i => i.Id != id && i.Name == input.Name);

            if (checkDuplicate)
                return Result.FromCode(ResultCode.DuplicateRecord);

            await _noticeTypeRepository.UpdateAsync(id, o => Task.FromResult(input.MapTo(o)));

            return Result.Ok();
        }

        /// <summary>
        /// 删除公告类型
        /// </summary>
        public async Task<Result> DeleteNoticeTypeAsync(int id)
        {
            await _noticeTypeRepository.DeleteAsync(id);
            return Result.Ok();
        }

        /// <summary>
        /// 获取公告类型分页列表
        /// </summary>
        public async Task<PageResult<NoticeTypeDto>> GetNoticeTypeAsync(QueryParameter<NoticeType> parameter)
        {
            var query = _noticeTypeRepository.AsNoTracking();
            var result = await query.ToQueryResultAsync(parameter, type => type.MapTo<NoticeTypeDto>());
            return result;
        }

        /// <summary>
        /// 查询公告类型列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">查询条件</param>
        /// <returns>列表结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<NoticeType> query = null)
        {
            return _noticeTypeRepository.AsNoTracking().ToPageResultAsync<NoticeType, TDto>(query);
        }

        /// <summary>
        /// 根据id获取公告类型
        /// </summary>
        public async Task<Result<NoticeTypeDto>> GetNoticeTypeById(int id)
        {
            var result =await _noticeTypeRepository.GetAsync(id);
            return Result.Ok(result.MapTo<NoticeTypeDto>());
        }

        /// <summary>
        /// 获取所有公告类型下拉列表
        /// </summary>
        /// <returns></returns>
        public Task<DropdownDto> GetNoticeTypeAsync()
        {
            return _noticeTypeRepository.AsNoTracking()
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(p => new DropdownItem() { Value = p.Id, Text = p.Name });
        }

        /// <summary>
        /// 根据条件查询公告类型
        /// </summary>
        public async Task<TDto> GetNoticeAsync<TDto>(IQuery<NoticeType> query)
        {
            return await _noticeTypeRepository.AsNoTracking().FirstOrDefaultAsync<NoticeType, TDto>(query);
        }

        /// <summary>
        /// 根据条件查询公告类型列表
        /// </summary>
        public async Task<IList<TDto>> GetNoticeListAsync<TDto>(IQuery<Core.Notice.NoticeType> query)
        {
            return await _noticeTypeRepository.AsNoTracking().ToListAsync<Core.Notice.NoticeType, TDto>(query);
        }

        #endregion
    }
}
