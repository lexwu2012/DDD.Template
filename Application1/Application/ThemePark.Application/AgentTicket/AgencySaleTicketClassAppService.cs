using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Core.CoreCache.CacheItem;
using System.Data.Entity;
using Abp.Extensions;
using Abp.Runtime.Caching;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CoreCache.Interfaces;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 代理商促销票类应用服务
    /// </summary>
    public class AgencySaleTicketClassAppService : ThemeParkAppServiceBase, IAgencySaleTicketClassAppService
    {
        #region Fields
        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketClassRepository;
        private readonly IRepository<ParkAgency> _parkAgencyRepository;
        private readonly IParkAgencyTypeGroupTypeAppService _parkAgencyTypeGroupTypeAppService;
        private readonly IRepository<Park> _parkRepository;
        private readonly IRepository<AgencyUser, long> _agencyUser;
        #endregion

        #region Cotr
        /// <summary>
        /// 构造依赖注入
        /// </summary>
        public AgencySaleTicketClassAppService(IRepository<AgencySaleTicketClass> agencySaleTicketClassRepository,
            IParkAgencyTypeGroupTypeAppService parkAgencyTypeGroupTypeAppService, IRepository<Park> parkRepository,
            IRepository<AgencyUser, long> agencyUser, IRepository<ParkAgency> parkAgencyRepository)
        {
            _agencySaleTicketClassRepository = agencySaleTicketClassRepository;
            _parkAgencyTypeGroupTypeAppService = parkAgencyTypeGroupTypeAppService;
            _parkRepository = parkRepository;
            _agencyUser = agencyUser;
            _parkAgencyRepository = parkAgencyRepository;
        }

        #endregion

        #region Public Methods       

        /// <summary>
        /// 批处理增加代理商促销门票
        /// </summary>
        /// <param name="inputs"></param>
        public async Task<Result> AddAgencySaleTicketClassListAsync(List<AgencySaleTicketClassSaveNewInput> inputs)
        {
            //获取日期有重叠的数据
            var duplicateRecordList = new List<AgencySaleTicketClassCheckExistedDto>();

            foreach (var input in inputs)
            {
                /*
                 *  长期促销只能有一条，短期促销的话多条记录中的时间不能重叠
                 * 
                 */
                //var agencySaleTicketClassSaveNewInput = inputs.First();
                var agencyTicketClassList = await GetAgencySaleTicketClassListAsync<AgencySaleTicketClassCheckExistedDto>(
                     new Query<AgencySaleTicketClass>(o => o.AgencyId == input.AgencyId
                     && o.AgencySaleTicketClassTemplateId == input.AgencySaleTicketClassTemplateId
                     && o.ParkId == input.ParkId));

                if (agencyTicketClassList.Count != 0)
                {
                    //给代理商促销票类赋上名称
                    //input.AgencySaleTicketClassName = agencyTicketClassList.First().AgencySaleTicketClassName;

                    foreach (var item in agencyTicketClassList)
                    {
                        //长期促销只能有一条记录
                        if (input.SaleStartDate == null && input.SaleEndDate == null && item.SaleStartDate == null && item.SaleEndDate == null)
                        {
                            duplicateRecordList.Add(item);
                            //有重复就跳出当前循环
                            break;
                        }

                        //短期促销的话多条记录中的时间不能重叠
                        if ((input.SaleStartDate >= item.SaleStartDate && input.SaleStartDate <= item.SaleEndDate)
                            || (input.SaleEndDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate)
                            || (input.SaleStartDate <= item.SaleStartDate && input.SaleEndDate >= item.SaleEndDate)
                            || (input.SaleStartDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate))
                        {
                            duplicateRecordList.Add(item);
                            break;
                        }
                    }
                }

            }

            //如果有重复的数据，将返回重复的数据到客户端
            if (duplicateRecordList.Count != 0)
            {
                string message = string.Empty;

                foreach (var duplicateRecord in duplicateRecordList)
                {
                    message += string.Format("{0}-{1}-{2},", duplicateRecord.ParkName, duplicateRecord.AgencyName, duplicateRecord.AgencySaleTicketClassTemplateName);
                }

                return Result.FromCode(ResultCode.DuplicateRecord, message.Substring(0, message.LastIndexOf(",", StringComparison.Ordinal)) + "<br>记录已存在.");
            }

            foreach (var input in inputs)
            {
                var entity = input.MapTo<AgencySaleTicketClass>();

                await _agencySaleTicketClassRepository.InsertAsync(entity);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 删除代理商促销门票
        /// </summary>
        /// <param name="id"></param>
        public async Task<Result> DeleteAgencySaleTicketClassAsync(int id)
        {
            await _agencySaleTicketClassRepository.DeleteAsync(id);

            return Result.Ok();
        }

        /// <summary>
        /// 更新代理商促销门票
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        public async Task<Result> UpdateAgencySaleTicketClassAsync(int id, AgencySaleTicketClassUpdateInput input)
        {
            var self = await _agencySaleTicketClassRepository.AsNoTrackingAndInclude(m => m.ParkSaleTicketClass).FirstOrDefaultAsync(p => p.Id == id);

            //查询排除掉本身记录的其他记录(不用 模板id做条件，判断的条件是代理商的某个基础票类在同一个公园时间不能重叠)
            var agencyTicketClassList = await GetAgencySaleTicketClassListAsync<AgencySaleTicketClassCheckExistedDto>(
                new Query<AgencySaleTicketClass>(o => o.Id != id
                && o.AgencyId == self.AgencyId
                && o.ParkSaleTicketClass.TicketClassId == self.ParkSaleTicketClass.TicketClassId
                && o.GroupTypeId == self.GroupTypeId
                && o.ParkId == self.ParkId
                && o.Status == TicketClassStatus.Sailing));

            //有其他记录
            if (agencyTicketClassList.Count != 0)
            {
                foreach (var item in agencyTicketClassList)
                {
                    //长期促销、同一促销不同时间
                    if (input.SaleStartDate >= item.SaleStartDate && input.SaleStartDate <= item.SaleEndDate
                            || (input.SaleEndDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate)
                            || (input.SaleStartDate <= item.SaleStartDate && input.SaleEndDate >= item.SaleEndDate)
                            || (input.SaleStartDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate))
                    {
                        return Result.FromCode(ResultCode.DuplicateRecord);
                    }
                }
            }

            await _agencySaleTicketClassRepository.UpdateAsync(id, m => Task.FromResult(input.MapTo(m)));

            return Result.Ok();
        }

        /// <summary>
        /// 获取促销票类信息
        /// </summary>
        public async Task<TDto> GetAgencySaleTicketClassAsync<TDto>(IQuery<AgencySaleTicketClass> query)
        {
            return await _agencySaleTicketClassRepository.AsNoTracking().FirstOrDefaultAsync<AgencySaleTicketClass, TDto>(query);
        }

        /// <summary>
        /// 获取促销票类信息
        /// </summary>
        public async Task<IList<TDto>> GetAgencySaleTicketClassListAsync<TDto>(IQuery<AgencySaleTicketClass> query)
        {
            return await _agencySaleTicketClassRepository.GetAll().ToListAsync<AgencySaleTicketClass, TDto>(query);
        }

        /// <summary>
        /// 获取代理商促销票类下拉列表
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<DropdownDto> GetAgencySaleTicketClassDropdownAsync(Expression<Func<AgencySaleTicketClass, bool>> exp)
        {
            return await _agencySaleTicketClassRepository.AsNoTracking().Where(exp).OrderBy(o => o.Id)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.AgencySaleTicketClassName, Value = o.Id });
        }

        /// <summary>
        /// 根据代理商ID获取团体售票的可售票类
        /// </summary>
        /// <returns>团体售票的可售票类</returns>
        public async Task<Result<List<AgencySaleTicketClassCacheItem>>> GetAgencySaleTicket4WindowAsync(GetAgencySaleTicketByWindowInput input)
        {
            var parkAgency = await _parkAgencyRepository.FirstOrDefaultAsync(p => p.AgencyId == input.AgencyId
                    && p.ParkId == input.ParkId && p.Status == ParkAgencyStatus.Valid);

            if (parkAgency == null)
            {
                return Result.FromError<List<AgencySaleTicketClassCacheItem>>("公园代理商为空");
            }
            else
            {
                if (!DateTime.Now.Date.IsBetween(parkAgency.StartDateTime, parkAgency.ExpirationDateTime))
                    return Result.FromError<List<AgencySaleTicketClassCacheItem>>("公园代理商不在有效期内");
            }

            var date = input.Plandate?.Date ?? DateTime.Now.Date;
            var dataList = await _agencySaleTicketClassRepository.GetAllListAsync(m => m.AgencyId == input.AgencyId
                                                       && m.GroupTypeId == input.GroupTypeId
                                                       && m.AgencySaleTicketClassTemplate.ParkId == input.ParkId
                                                       && ((date >= m.SaleStartDate && date <= m.SaleEndDate) || (m.SaleStartDate == null && m.SaleEndDate == null))
                                                       && m.Status == TicketClassStatus.Sailing);

            var items = dataList.Select(data => new AgencySaleTicketClassCacheItem()
            {
                Id = data.Id,
                Price = data.Price,
                SalePrice = data.SalePrice,
                TicketClassName = data.AgencySaleTicketClassTemplate.ParkSaleTicketClass.SaleTicketClassName,
                TicketClassMode = data.AgencySaleTicketClassTemplate.ParkSaleTicketClass.TicketClass.TicketClassMode,
                //OutTicketType = data.AgencySaleTicketClassTemplate.ParkSaleTicketClass.TicketClass.OutTicketType,
                GroupTypeId = data.GroupTypeId,

                SerialNumber = data.ParkSaleTicketClass.TicketClass.TicketType.SerialNumber,
            }).OrderBy(m => m.SerialNumber).ToList();

            return Result.FromData(items);
        }

        /// <summary>
        /// 获取票类可入园人数
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        public async Task<Result<int>> GetTicketClassPersonsAsync(int id)
        {
            var persons = await _agencySaleTicketClassRepository.GetAll()
                    .Where(o => o.Id == id)
                    .Select(o => o.ParkSaleTicketClass.TicketClass.TicketType.Persons)
                    .FirstOrDefaultAsync();

            if (persons == default(int))
            {
                return Result.FromCode<int>(ResultCode.NoRecord);
            }

            return Result.FromData(persons);
        }

        /// <summary>
        /// 获取票类名称
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        public Task<string> GetAgencySaleTicketClassNameAsync(int id)
        {
            return _agencySaleTicketClassRepository.AsNoTracking().Where(o => o.Id == id)
                .Select(o => o.AgencySaleTicketClassName).SingleAsync();
        }

        /// <summary>
        /// 通过ID从缓存中获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AgencySaleTicketClassCacheItem> GetByIdAsync(int id)
        {
            return (await _agencySaleTicketClassRepository.GetAsync(id)).MapTo<AgencySaleTicketClassCacheItem>();
        }

        /// <summary>
        /// 根据代理商Id获取团体类型
        /// </summary>
        /// <param name="agencyTypeId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<DropdownDto> GetGroupTypeByAgencyAndParkIdAsync(int agencyTypeId, int parkId)
        {
            return await _parkAgencyTypeGroupTypeAppService.GetGroupTypeDropdownByAgencyTypeIdAndParkIdAsync(m => m.ParkId == parkId && m.AgencyTypeId == agencyTypeId);
        }


        /// <summary>
        /// 获取代理商促销票类列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<AgencySaleTicketClass> query = null)
        {
            //var sessionParks = AbpSession.Parks;
            //var data = _agencySaleTicketClassRepository.AsNoTracking().Where(m => sessionParks.Contains(m.ParkId));

            return _agencySaleTicketClassRepository.AsNoTracking().ToPageResultAsync<AgencySaleTicketClass, TDto>(query);
        }

        /// <summary>
        /// 通过Userid获取代理商可售票类
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<List<ApiAgencySaleTicketTypeDto>>> GetAgencySaleTicketTypeTask(long userId)
        {
            var agencyId = (await _agencyUser.GetAsync(userId)).AgencyId;

            var ticketTypes = await _agencySaleTicketClassRepository.GetAllListAsync(
                p => p.AgencyId == agencyId && p.Status == TicketClassStatus.Sailing
                && ((p.AgencySaleTicketClassTemplate.AgencyType.DefaultAgencyType == DefaultAgencyType.Ota) || (p.AgencySaleTicketClassTemplate.AgencyType.DefaultAgencyType == DefaultAgencyType.OwnOta)));

            List<ApiAgencySaleTicketTypeDto> agencySaleTicketTypes = new List<ApiAgencySaleTicketTypeDto>();
            foreach (var type in ticketTypes)
            {
                var item = type.MapTo<ApiAgencySaleTicketTypeDto>();
                if (type?.AgencySaleTicketClassTemplate?.AgencyType?.DefaultAgencyType == DefaultAgencyType.Ota)
                {
                    item.Price = type.SettlementPrice;
                    item.StandPrice = type.SalePrice;
                }
                else if (type?.AgencySaleTicketClassTemplate?.AgencyType?.DefaultAgencyType == DefaultAgencyType.OwnOta)
                {
                    item.Price = type.SalePrice;
                    item.StandPrice = type.Price;
                }
                else
                {
                    return Result.FromCode<List<ApiAgencySaleTicketTypeDto>>(ResultCode.Fail);
                }
                var parkEntity = await _parkRepository.FirstOrDefaultAsync(type.ParkId);
                item.ParkCode = parkEntity.ParkCode;
                item.ParkName = parkEntity.ParkName;
                agencySaleTicketTypes.Add(item);
            }

            return Result.FromData(agencySaleTicketTypes);
        }

        /// <summary>
        /// 根据代理商Id和团体类型Id获取公园列表（旅行社用到）
        /// </summary>
        /// <param name="agencyId"></param>
        /// <param name="groupTypeId"></param>
        /// <returns></returns>
        public async Task<DropdownDto> GetParksByActiveAgencyIdAndGroupTypeIdAsync(int agencyId, int groupTypeId)
        {
            List<AgencySaleTicketClass> parkResult = new List<AgencySaleTicketClass>();
            var agencySaleTicketClassList = _agencySaleTicketClassRepository.GetAll().Where(m => m.AgencyId == agencyId && m.GroupTypeId == groupTypeId && m.Park.IsActive).ToList();
            foreach (var agencySaleTicketClass in agencySaleTicketClassList)
            {
                var parkAgency = await _parkAgencyRepository.FirstOrDefaultAsync(p => p.AgencyId == agencySaleTicketClass.AgencyId
                    && p.ParkId == agencySaleTicketClass.ParkId && p.Status == ParkAgencyStatus.Valid);

                if (parkAgency != null && DateTime.Now.Date.IsBetween(parkAgency.StartDateTime, parkAgency.ExpirationDateTime))
                {
                    if (!parkResult.Select(p => p.ParkId).Contains(agencySaleTicketClass.ParkId))
                    {
                        parkResult.Add(agencySaleTicketClass);
                    }
                }
            }

            return parkResult.Select(m => new DropdownItem { Value = m.ParkId, Text = m.Park.ParkName }).Distinct().ToDropdownDto();

            //return await _agencySaleTicketClassRepository.GetAll().Where(m => m.AgencyId == agencyId && m.GroupTypeId == groupTypeId && m.Park.IsActive)
            //    .Select(m => new DropdownItem { Value = m.ParkId, Text = m.Park.ParkName })
            //    .Distinct()
            //    .ToDropdownDtoAsync();
        }

        #endregion
    }


    /// <summary>
    /// 比较长期票是否与活动票重复
    /// </summary>
    public class IsLongTimeTicketNeed : IEqualityComparer<AgencySaleTicketClass>
    {
        public bool Equals(AgencySaleTicketClass x, AgencySaleTicketClass y)
        {
            if (x == null)
                return y == null;
            return x.Id != y.Id && x.ParkSaleTicketClass.TicketClassId == y.ParkSaleTicketClass.TicketClassId;
        }

        public int GetHashCode(AgencySaleTicketClass obj)
        {
            if (obj == null)
                return 0;
            return obj.Id.GetHashCode();
        }
    }
}
