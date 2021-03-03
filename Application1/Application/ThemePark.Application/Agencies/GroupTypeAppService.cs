using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.Agencies
{
    class GroupTypeAppService : ThemeParkAppServiceBase, IGroupTypeAppService
    {
        #region Fields

        private readonly IRepository<GroupType> _groupTypeRepository;

        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketRepository;

        #endregion

        #region Ctor

        public GroupTypeAppService(IRepository<GroupType> groupTypeRepository, IRepository<AgencySaleTicketClass> agencySaleTicketRepository)
        {
            _groupTypeRepository = groupTypeRepository;
            _agencySaleTicketRepository = agencySaleTicketRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 增加新的带队类型,并返回该新增实体
        /// </summary>
        public async Task<Result<GroupTypeDto>> AddGroupTypeAsync(GroupTypeInput input)
        {
            var check = _groupTypeRepository.GetAll().Any(o => o.TypeName == input.TypeName);

            if (check)
                return Result.FromCode<GroupTypeDto>(ResultCode.DuplicateRecord);

            var groupType = input.MapTo<GroupType>();
            await _groupTypeRepository.InsertAndGetIdAsync(groupType);

            return Result.Ok(groupType.MapTo<GroupTypeDto>());
        }

        /// <summary>
        /// 更新带队类型信息
        /// </summary>
        public async Task<Result> UpdateGroupTypeAsync(int id, GroupTypeInput input)
        {
            var check = _groupTypeRepository.GetAll().Any(i => i.Id == id);
            //查看是否存在该数据
            if (!check)
                return Result.FromCode(ResultCode.NoRecord);
            var checkDuplicate = _groupTypeRepository.GetAll().Any(i => i.Id != id && i.TypeName == input.TypeName);

            if (checkDuplicate)
                return Result.FromCode(ResultCode.DuplicateRecord);

            await _groupTypeRepository.UpdateAsync(id, o => Task.FromResult(input.MapTo(o)));

            return Result.Ok();
        }

        /// <summary>
        /// 根据Id删除带队类型
        /// </summary>
        public async Task<Result> DeleteGroupTypeByIdAsync(int id)
        {
            await _groupTypeRepository.DeleteAsync(o => o.Id == id);

            return Result.Ok();
        }

        /// <summary>
        /// 获取带队类型下拉列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetGroupTypesDropdownAsync()
        {
            return _groupTypeRepository.GetAll()
                .OrderBy(o => o.Id)
                .ToDropdownDtoAsync(type => new DropdownItem() { Text = type.TypeName, Value = type.Id });
        }

        /// <summary>
        /// 获取列表展示内容
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<GroupType> query = null)
        {
            return _groupTypeRepository.AsNoTracking().ToPageResultAsync<GroupType, TDto>(query);
        }

        /// <summary>
        /// 查询带队类型
        /// </summary>
        public async Task<TDto> GetGroupTypeAsync<TDto>(IQuery<GroupType> query)
        {
            return await _groupTypeRepository.AsNoTracking().FirstOrDefaultAsync<GroupType, TDto>(query);
        }

        /// <summary>
        /// 查询带队类型列表
        /// </summary>
        public async Task<IList<TDto>> GetGroupTypeListAsync<TDto>(IQuery<GroupType> query)
        {
            return await _groupTypeRepository.AsNoTracking().ToListAsync<GroupType, TDto>(query);
        }


        /// <summary>
        /// 获取所有的带队类型
        /// </summary>
        /// <returns></returns>
        public async Task<DropdownDto> GetAllGroupTypesAsync()
        {
            return await _groupTypeRepository.AsNoTracking()
                .OrderBy(o => o.Id)
                .Select(o => new DropdownItem() { Text = o.TypeName, Value = o.Id })
                .ToDropdownDtoAsync();
        }

        /// <summary>
        /// 获取当前登录代理商的带队类型
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public async Task<DropdownDto> GetAgencyGroupTypesDropdownAsync(int? agencyId)
        {
            var types = await _agencySaleTicketRepository.AsNoTracking().Where(m =>m.AgencyId == agencyId.Value && m.Status == Core.BasicTicketType.TicketClassStatus.Sailing)
                .Select(o => o.GroupTypeId)
                .Distinct()
                .ToListAsync();
            return await _groupTypeRepository.AsNoTracking()
                .Where(o => types.Contains(o.Id))
                .OrderBy(o => o.Id).ToDropdownDtoAsync(o => new DropdownItem() { Text = o.TypeName, Value = o.Id });
        }
        #endregion
    }
}
