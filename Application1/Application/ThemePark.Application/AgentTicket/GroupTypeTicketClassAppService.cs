using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.ApplicationDto.AgentTicket;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 可带队类型设置AppService
    /// </summary>
    public class GroupTypeTicketClassAppService : ThemeParkAppServiceBase, IGroupTypeTicketClassAppService
    {

        #region Fields

        private readonly IRepository<GroupTypeTicketClass> _groupTypeTicketClassRepository;
        private readonly IRepository<TicketClass> _ticketClassRepository;
        private readonly IRepository<GroupType> _groupTypeRepository;
        private readonly IRepository<Park> _parkRepository;

        #endregion

        #region Ctor
        /// <summary>
        /// 可带队类型构造函数
        /// </summary>
        /// <param name="groupTypeTicketClassRepository"></param>
        public GroupTypeTicketClassAppService(IRepository<GroupTypeTicketClass> groupTypeTicketClassRepository,
            IRepository<TicketClass> ticketClassRepository, IRepository<GroupType> groupTypeRepository, IRepository<Park> parkRepository)
        {
            _groupTypeTicketClassRepository = groupTypeTicketClassRepository;
            _ticketClassRepository = ticketClassRepository;
            _groupTypeRepository = groupTypeRepository;
            _parkRepository = parkRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 增加新的团队可售票类型
        /// </summary>
        public async Task<Result> AddGroupTypeTicketClassAsync(List<AddGroupTypeTicketClassInput> inputs)
        {
            foreach (var input in inputs)
            {
                var exist =
                    _groupTypeTicketClassRepository.GetAll()
                        .Any(o => o.ParkId == input.ParkId && o.GroupTypeId == input.GroupTypeId);

                if (exist)
                {
                    return Result.FromCode(ResultCode.DuplicateRecord);
                }
            }

            foreach (var input in inputs)
            {
                var entity = input.MapTo<GroupTypeTicketClass>();

                await _groupTypeTicketClassRepository.InsertAsync(entity);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 更新团队可售票类型信息
        /// </summary>
        public async Task<Result> UpdateGroupTypeTicketClassAsync(int parkId, int groupTypeId, GroupTypeTicketClassUpdateInput input)
        {
            var ids = _groupTypeTicketClassRepository.AsNoTracking().Where(g => g.ParkId == parkId && g.GroupTypeId == groupTypeId).Select(o => o.TicketClassId).ToList();
            if (ids.Count == 0)
            {
                return Result.FromCode(ResultCode.NoRecord);
            }
            //获取所有选中的票类
            var checkedIds = new List<int>();

            foreach (var ticketClassId in input.TicketClassId)
            {
                checkedIds.Add(ticketClassId);
                //判断是否已存在
                //若存在，则不做修改
                //若不存在，则添加
                if (!ids.Contains(ticketClassId))
                {
                    GroupTypeTicketClass classItem = new GroupTypeTicketClass
                    {
                        ParkId = parkId,
                        GroupTypeId = groupTypeId,
                        TicketClassId = ticketClassId
                    };
                    await _groupTypeTicketClassRepository.InsertAsync(classItem);
                }

            }

            //删除多余项
            foreach (var id in ids)
            {
                if (!checkedIds.Contains(id))
                {
                    await _groupTypeTicketClassRepository.DeleteAsync(o => o.GroupTypeId == groupTypeId && o.ParkId == parkId && o.TicketClassId == id);
                }
            }
            return Result.Ok();
        }

        /// <summary>
        /// 获取团队可售票分页列表
        /// </summary>
        public async Task<PageResult<GroupTypeTicketClassList>> GetAssembleDataByParkAndGroupTypeId(IPageQuery<GroupTypeTicketClass> query)
        {
            //搜索分页数据：根据公园ID和团队类型ID分组
            var group = _groupTypeTicketClassRepository.GetAll().Where(query)
                .GroupBy(p => new { p.ParkId, p.GroupTypeId });
            var data = group.Select(p => new GroupTypeTicketClassList { ParkId = p.Key.ParkId, GroupTypeId = p.Key.GroupTypeId });

            var total = await data.CountAsync();
            var result = await data.OrderBy(query).PageBy(query, "ParkId asc").ToListAsync();

            var tickets = await group.SelectMany(p => p.Select(o => new { o.ParkId, o.GroupTypeId, o.TicketClassId, o.TicketClass.TicketClassName }))
                .Distinct().ToListAsync();

            var parkIds = result.Select(p => p.ParkId).Distinct().ToArray();
            var groupTypeIds = result.Select(p => p.GroupTypeId).Distinct().ToArray();
            var parks = await _parkRepository.AsNoTracking().Where(p => parkIds.Contains(p.Id)).Select(p => new { p.Id, p.ParkName }).ToListAsync();
            var groupTypes = await _groupTypeRepository.AsNoTracking().Where(p => groupTypeIds.Contains(p.Id)).Select(p => new { p.Id, p.TypeName }).ToListAsync();

            foreach (var item in result)
            {
                item.ParkName = parks.FirstOrDefault(p => p.Id == item.ParkId)?.ParkName;
                item.GroupTypeName = groupTypes.FirstOrDefault(p => p.Id == item.GroupTypeId)?.TypeName;
                item.TicketClasses = tickets.Where(p => p.ParkId == item.ParkId && p.GroupTypeId == item.GroupTypeId)
                    .Select(p => p.TicketClassName)
                    .ToList();
            }

            return new PageResult<GroupTypeTicketClassList>(result, total, query);
        }


        /// <summary>
        /// 查询获取列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">查询条件</param>
        /// <returns>列表结果</returns>
        public async Task<PageResult<TDto>> GetPagedDataAsync<TDto>(IPageQuery<GroupTypeTicketClass> query = null)
        {
            return await _groupTypeTicketClassRepository.AsNoTracking().ToPageResultAsync<GroupTypeTicketClass, TDto>(query);
        }

        /// <summary>
        /// 根据旅游团类型Id、公园Id获取旅游团可售票类
        /// </summary>
        public async Task<IList<TicketClassDto>> GetTicketClassesByParkAndGroupTypeId(int parkId, int groupId)
        {
            var query = await _groupTypeTicketClassRepository.GetAllListAsync(g => g.ParkId == parkId && g.GroupTypeId == groupId);
            query.GroupBy(o => o.TicketClassId);
            query.OrderBy(o => o.TicketClassId);
            List<TicketClassDto> ticketClassDto = new List<TicketClassDto>();
            foreach (var groupItem in query)
            {
                if (groupItem.TicketClass == null || ticketClassDto.Any(m => m.Id == groupItem.TicketClass.Id))
                    continue;
                ticketClassDto.Add(groupItem.TicketClass.MapTo<TicketClassDto>());
            }
            if (ticketClassDto.Count == 0)
            {
                return null;
            }
            return ticketClassDto;
        }

        /// <summary>
        /// 删除团队可售票类型
        /// </summary>
        public async Task<Result> DeleteGroupTypeTicketClassAsync(int parkId, int groupTypeId)
        {
            await _groupTypeTicketClassRepository.DeleteAsync(g => g.ParkId == parkId && g.GroupTypeId == groupTypeId);
            return Result.Ok();
        }

        /// <summary>
        /// 获取所有旅游团可售票类
        /// </summary>
        public IList<GroupTypeTicketClassDto> GetGroupTypeTicketClasses()
        {
            var GroupTypeTicketClass = _groupTypeTicketClassRepository.GetAll();

            return GroupTypeTicketClass.MapTo<IList<GroupTypeTicketClassDto>>();
        }

        /// <summary>
        /// 根据条件搜索一条记录
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetGroupTypeTicketClassAsync<TDto>(IQuery<GroupTypeTicketClass> query)
        {
            return await _groupTypeTicketClassRepository.GetAll().FirstOrDefaultAsync<GroupTypeTicketClass, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取团体票类列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetGroupTypeTicketClassListAsync<TDto>(IQuery<GroupTypeTicketClass> query)
        {
            return await _groupTypeTicketClassRepository.GetAll().ToListAsync<GroupTypeTicketClass, TDto>(query);
        }

        #endregion
    }
}
