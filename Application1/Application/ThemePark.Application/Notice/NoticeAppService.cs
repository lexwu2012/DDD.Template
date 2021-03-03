using System;
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
    /// 公告Service
    /// </summary>
    public class NoticeAppService : ThemeParkAppServiceBase, INoticeAppService
    {
        #region Fields

        private readonly IRepository<ThemePark.Core.Notice.Notice> _noticeRepository;

        private readonly IRepository<NoticeType> _noticeTypeRepository;

        private readonly IRepository<ThemePark.Core.Agencies.ParkAgency> _parkAgencyRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造
        /// </summary>
        public NoticeAppService(IRepository<ThemePark.Core.Notice.Notice> noticeRepository, IRepository<NoticeType> noticeTypeRepository, IRepository<ThemePark.Core.Agencies.ParkAgency> parkAgencyRepository)
        {
            _noticeRepository = noticeRepository;
            _noticeTypeRepository = noticeTypeRepository;
            _parkAgencyRepository = parkAgencyRepository;
        }

        #endregion
        /// <summary>
        /// 将公园ID拼成string
        /// </summary>
        /// <param name="parkIdList"></param>
        /// <returns></returns>
        private string GetParkIds(List<int> parkIdList)
        {
            var list = parkIdList.Distinct().ToList();
            list.Sort((x, y) => x - y);

            var parkIds = string.Join(",", parkIdList);
            return "," + parkIds + ",";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parkIds"></param>
        /// <returns></returns>
        private List<string> GetParkPermissions(List<int> parkIds)
        {
            return AbpSession.Parks.Select(o => $",{o},").ToList();
        }

        /// <summary>
        /// 添加公告
        /// </summary>
        public async Task<Result> AddNoticeAsync(AddNoticeInput input)
        {
            var notice = input.MapTo<Core.Notice.Notice>();
            notice.ParkIds = GetParkIds(input.ParkIdList);
            await _noticeRepository.InsertAsync(notice);
            return Result.Ok();
        }

        /// <summary>
        /// 浏览次数+1
        /// </summary>
        public async Task<Result> AddTimes(int id)
        {
            var notice = await _noticeRepository.FirstOrDefaultAsync(id);
            if (notice == null)
                return Result.FromCode(ResultCode.NoRecord);
            notice.Times++;
            await _noticeRepository.UpdateAsync(notice);
            return Result.Ok();
        }

        /// <summary>
        /// 修改公告
        /// </summary>
        public async Task<Result> UpdateNoticeAsync(int id, UpdateNoticeInput input)
        {
            var check = _noticeRepository.GetAll().FirstOrDefault(o => o.Id == id);
            if (check == null)
                return Result.FromCode(ResultCode.NoRecord);

            input.MapTo(check);
            check.ParkIds = GetParkIds(input.ParkIdList);

            await _noticeRepository.UpdateAsync(check);
            return Result.Ok();

        }

        /// <summary>
        /// 删除公告
        /// </summary>
        public async Task<Result> DeleteNoticeAsync(int id)
        {
            await _noticeRepository.DeleteAsync(id);
            return Result.Ok();

        }

        /// <summary>
        /// 查询公告
        /// </summary>
        public async Task<PageResult<NoticeDto>> GetNoticeAsync(QueryParameter<ThemePark.Core.Notice.Notice> param)
        {
            var query = _noticeRepository.AsNoTracking();
            var result = await query.ToQueryResultAsync(param, type => type.MapTo<NoticeDto>());
            return result;
        }

        /// <summary>
        /// 查询公告
        /// </summary>
        public Task<PageResult<TDto>> GetNoticeByPageAsync<TDto>(IPageQuery<Core.Notice.Notice> query = null)
        {
            return _noticeRepository.AsNoTracking().Where(o => o.ReleaseTime < DateTime.Now).ToPageResultAsync<Core.Notice.Notice, TDto>(query, "SerialNumber asc, ReleaseTime desc");
        }

        /// <summary>
        /// 查询旅行社系统的公告
        /// </summary>
        public Task<PageResult<TDto>> GetNoticeBySystemTypeIdAsync<TDto>(IPageQuery<Core.Notice.Notice> query = null)
        {
            var parks = GetParkPermissions(AbpSession.Parks);
            var result = _noticeRepository.GetAll()
                .Where(o => o.SystemTypeId.HasValue && o.SystemTypeId.Value.HasFlag(SystemType.AgencyReserve)
                && parks.Any(p => o.ParkIds.Contains(p)) && o.ReleaseTime < DateTime.Now)
                .ToPageResultAsync<Core.Notice.Notice, TDto>(query, "SerialNumber asc, ReleaseTime desc");
            return result;
        }

        /// <summary>
        /// 查询中心系统的公告
        /// </summary>
        public Task<PageResult<TDto>> GetNoticeBySystemTypeAsync<TDto>(IPageQuery<Core.Notice.Notice> query = null)
        {
            var parks = GetParkPermissions(AbpSession.Parks);
            var result = _noticeRepository.GetAll().Where(o => o.SystemTypeId.HasValue && o.SystemTypeId.Value.HasFlag(SystemType.ThemePark) && parks.Any(p => o.ParkIds.Contains(p)) && o.ReleaseTime < DateTime.Now).ToPageResultAsync<Core.Notice.Notice, TDto>(query, "SerialNumber asc, ReleaseTime desc");
            return result;
        }

        /// <summary>
        /// 根据ID搜索公告
        /// </summary>
        public async Task<TDto> GetNoticeAsync<TDto>(IQuery<Core.Notice.Notice> query)
        {
            return await _noticeRepository.AsNoTracking().FirstOrDefaultAsync<Core.Notice.Notice, TDto>(query);
        }

        /// <summary>
        /// 根据条件查询公告列表
        /// </summary>
        public async Task<IList<TDto>> GetNoticeListAsync<TDto>(IQuery<Core.Notice.Notice> query, string defaultSort = "Id Desc", int dataAmount = 10)
        {
            return await _noticeRepository.AsNoTracking().ToListAsync<Core.Notice.Notice, TDto>(query, null, dataAmount, defaultSort);
        }
    }
}
