using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Common;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 代理商促销模板应用服务
    /// </summary>
    public class AgencySaleTicketClassTemplateAppService : ThemeParkAppServiceBase, IAgencySaleTicketClassTemplateAppService
    {
        #region Fields
        private readonly IRepository<AgencySaleTicketClassTemplate> _agencySaleTicketClassTemplateRepository;
        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketClassRepository;
        private readonly IAgencySaleTicketClassAppService _agencySaleTicketClassAppService;
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="agencySaleTicketClassTemplateRepository"></param>
        /// <param name="agencySaleTicketClassRepository"></param>
        public AgencySaleTicketClassTemplateAppService(IRepository<AgencySaleTicketClassTemplate> agencySaleTicketClassTemplateRepository, IRepository<AgencySaleTicketClass> agencySaleTicketClassRepository, IAgencySaleTicketClassAppService agencySaleTicketClassAppService)
        {
            _agencySaleTicketClassTemplateRepository = agencySaleTicketClassTemplateRepository;
            _agencySaleTicketClassRepository = agencySaleTicketClassRepository;
            _agencySaleTicketClassAppService = agencySaleTicketClassAppService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 增加代理商促销门票
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="agencySaleTicketClassSaveNewInputList"></param>
        /// <param name="ticketClassId"></param>
        public async Task<Result> AddAgencySaleTicketClassTemplateAsync(AgencySaleTicketClassTemplate dto, 
            List<AgencySaleTicketClassSaveNewInput> agencySaleTicketClassSaveNewInputList, int ticketClassId)
        {
            if (dto.SaleStartDate != null && dto.SaleEndDate != null)
            {
                if (DateTime.Compare(dto.SaleStartDate.Value, dto.SaleEndDate.Value) > 0)
                {
                    return Result.FromError("促销开始日期必须小于促销结束日期.");
                }
            }

            //获取日期有重叠的数据
            //var duplicateRecordList = new List<AgencySaleTicketClassCheckExistedDto>();

            foreach (var input in agencySaleTicketClassSaveNewInputList)
            {
                /*
                 *  长期促销只能有一条，短期促销的话多条记录中的时间不能重叠
                 * 
                 */
                //判断的条件是代理商的某个基础票类在同一个公园时间不能重叠
                var agencyTicketClassList = await _agencySaleTicketClassAppService.GetAgencySaleTicketClassListAsync<AgencySaleTicketClassCheckExistedDto>(
                    new Query<AgencySaleTicketClass>(o => o.AgencyId == input.AgencyId
                    && o.ParkSaleTicketClass.TicketClassId == ticketClassId
                    && o.GroupTypeId == dto.GroupTypeId
                    && o.ParkId == input.ParkId
                    && o.Status == TicketClassStatus.Sailing));


                //给代理商促销票类赋上名称
                //input.AgencySaleTicketClassName = agencyTicketClassList.First().AgencySaleTicketClassName;

                foreach (var item in agencyTicketClassList)
                {
                    //长期促销只能有一条记录
                    if (input.SaleStartDate == null && input.SaleEndDate == null && item.SaleStartDate == null && item.SaleEndDate == null)
                    {
                        //duplicateRecordList.Add(item);
                        ////有重复就跳出当前循环
                        //break;
                        var message = $"{item.ParkName}-{item.AgencyName}-{item.AgencySaleTicketClassName}";
                        return Result.FromCode(ResultCode.DuplicateRecord, message + "<br />记录存在重复时间");
                    }

                    //短期促销的话多条记录中的时间不能重叠
                    if ((input.SaleStartDate >= item.SaleStartDate && input.SaleStartDate <= item.SaleEndDate)
                        || (input.SaleEndDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate)
                        || (input.SaleStartDate <= item.SaleStartDate && input.SaleEndDate >= item.SaleEndDate)
                        || (input.SaleStartDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate))
                    {
                        //duplicateRecordList.Add(item);
                        //break;
                        var message = $"{item.ParkName}-{item.AgencyName}-{item.AgencySaleTicketClassName}";
                        return Result.FromCode(ResultCode.DuplicateRecord, message + "<br />记录存在重复时间");
                    }
                }
            }

            //新增模板
            var id = await _agencySaleTicketClassTemplateRepository.InsertAndGetIdAsync(dto.MapTo<AgencySaleTicketClassTemplate>());

            //给代理商促销票类赋上代理商促销票类模板Id
            agencySaleTicketClassSaveNewInputList.ForEach(m => m.AgencySaleTicketClassTemplateId = id);

            //新增代理商促销票类
            foreach (var input in agencySaleTicketClassSaveNewInputList)
            {
                await _agencySaleTicketClassRepository.InsertAsync(input.MapTo<AgencySaleTicketClass>());
            }

            return Result.Ok();
        }

        /// <summary>
        /// 删除代理商促销门票
        /// </summary>
        /// <param name="id"></param>
        public async Task<Result> DeleteAgencySaleTicketClassTemplateAsync(int id)
        {
            //判断是否有代理商促销票类
            var agencySaleTicketClassList = _agencySaleTicketClassRepository.GetAll().Where(m => m.AgencySaleTicketClassTemplateId == id);

            if (agencySaleTicketClassList.Any())
            {
                return Result.FromError("需要删除该模板的话，需要先删除关联此模板的代理商促销票类");
            }

            await _agencySaleTicketClassTemplateRepository.DeleteAsync(id);

            return Result.Ok();
        }

        /// <summary>
        /// 更新代理商促销门票
        /// </summary>
        /// <param name="input"></param>
        public async Task<Result> UpdateAgencySaleTicketClassTemplateAsync(UpdateAgencySaleTicketClassTemplateInput input)
        {
            //1. 更新代理商促销票类模板
            var agencySaleTicketClassTemplateUpdateInput4Save = input.MapTo<AgencySaleTicketClassTemplateUpdateInput4Save>();

            if (DateTime.Compare(agencySaleTicketClassTemplateUpdateInput4Save.SaleStartDate, agencySaleTicketClassTemplateUpdateInput4Save.SaleEndDate) > 0)
            {
                return Result.FromError("促销开始日期必须小于促销结束日期");
            }

            //1. 更新代理商促销票类
            var list = new List<AgencySaleTicketClassSaveNewInput>();
            
            //1）如果选择了覆盖，则勾选上的已有数据进行覆盖，勾选上的新增数据添加
            if (input.IsOverRide)
            {
                //添加勾选上的代理商记录
                foreach (var item in input.CombineList)
                {
                    if (!item.Checked)
                        continue;

                    var agencySaleTicketClass = await _agencySaleTicketClassRepository.GetAllIncluding(m => m.ParkSaleTicketClass).FirstOrDefaultAsync(
                        m => m.AgencySaleTicketClassTemplateId == input.Id
                        && m.AgencyId == item.AgencyId);

                    //已经存在代理商促销票类，则更改
                    if (agencySaleTicketClass != null)
                    {
                        //判断的条件是代理商的某个基础票类在同一个公园时间不能重叠
                        var agencyTicketClassList = await _agencySaleTicketClassAppService.GetAgencySaleTicketClassListAsync<AgencySaleTicketClassCheckExistedDto>(
                            new Query<AgencySaleTicketClass>(o => o.Id != agencySaleTicketClass.Id
                            && o.AgencyId == agencySaleTicketClass.AgencyId
                            && o.ParkSaleTicketClass.TicketClassId == agencySaleTicketClass.ParkSaleTicketClass.TicketClassId
                            && o.GroupTypeId == agencySaleTicketClass.GroupTypeId
                            && o.ParkId == agencySaleTicketClass.ParkId
                            && o.Status == TicketClassStatus.Sailing));

                        foreach (var agencyTicketClass in agencyTicketClassList)
                        {
                            //短期促销的话多条记录中的时间不能重叠
                            if ((input.SaleStartDate >= agencyTicketClass.SaleStartDate && input.SaleStartDate <= agencyTicketClass.SaleEndDate)
                                || (input.SaleEndDate >= agencyTicketClass.SaleStartDate && input.SaleEndDate <= agencyTicketClass.SaleEndDate)
                                || (input.SaleStartDate <= agencyTicketClass.SaleStartDate && input.SaleEndDate >= agencyTicketClass.SaleEndDate)
                                || (input.SaleStartDate >= agencyTicketClass.SaleStartDate && input.SaleEndDate <= agencyTicketClass.SaleEndDate))
                            {
                                var message =
                                    $"{agencyTicketClass.ParkName}-{agencyTicketClass.AgencyName}-{agencyTicketClass.AgencySaleTicketClassName}";
                                return Result.FromCode(ResultCode.DuplicateRecord, message + "<br />记录存在重复时间");
                            }
                        }

                        var agencyTicketClassInput = new UpdateTemplate4AgencySaleTicketClassInput
                        {
                            AgencySaleTicketClassName = input.AgencySaleTicketClassName,
                            Price = input.Price,
                            SaleEndDate = input.SaleEndDate,
                            SaleStartDate = input.SaleStartDate,
                            SalePrice = input.SalePrice,
                            SettlementPrice = input.SettlementPrice,
                            Status = input.Status,
                            ParkSettlementPrice = input.ParkSettlementPrice,
                            Remark = input.Remark
                        };
                        //对应的代理商促销票类也要更改
                        await _agencySaleTicketClassRepository.UpdateAsync(agencySaleTicketClass.Id, m => Task.FromResult(agencyTicketClassInput.MapTo(m)));
                    }
                    else
                    {
                        //新数据
                        var addInput = new AgencySaleTicketClassSaveNewInput()
                        {
                            AgencyId = item.AgencyId,
                            ParkSaleTicketClassId = input.ParkSaleTicketClassId,
                            ParkId = input.ParkId,
                            AgencySaleTicketClassTemplateId = input.Id,
                            AgencySaleTicketClassName = input.AgencySaleTicketClassName,
                            Price = input.Price,
                            SalePrice = input.SalePrice,
                            SettlementPrice = input.SettlementPrice,
                            ParkSettlementPrice = input.ParkSettlementPrice,
                            Status = input.Status,
                            Remark = input.Remark,
                            SaleEndDate = input.SaleEndDate,
                            SaleStartDate = input.SaleStartDate,
                            GroupTypeId = input.GroupTypeId
                        };
                        list.Add(addInput);
                    }
                }
            }
            else
            {
                //2）不覆盖，则勾选上的已有数据不进行覆盖，新增数据则添加
                //添加勾选上的代理商记录
                foreach (var item in input.CombineList)
                {
                    if (!item.Checked)
                        continue;

                    var agencySaleTicketClass = await _agencySaleTicketClassRepository.FirstOrDefaultAsync(
                        m => m.AgencySaleTicketClassTemplateId == input.Id && m.AgencyId == item.AgencyId);

                    //已经存在代理商促销票类，不进行覆盖
                    if (agencySaleTicketClass != null)
                        continue;

                    //新数据
                    var addInput = new AgencySaleTicketClassSaveNewInput()
                    {
                        AgencyId = item.AgencyId,
                        ParkSaleTicketClassId = input.ParkSaleTicketClassId,
                        ParkId = input.ParkId,
                        AgencySaleTicketClassTemplateId = input.Id,
                        AgencySaleTicketClassName = input.AgencySaleTicketClassName,
                        Price = input.Price,
                        SalePrice = input.SalePrice,
                        SettlementPrice = input.SettlementPrice,
                        ParkSettlementPrice = input.ParkSettlementPrice,
                        Status = input.Status,
                        Remark = input.Remark,
                        SaleEndDate = input.SaleEndDate,
                        SaleStartDate = input.SaleStartDate,
                        GroupTypeId = input.GroupTypeId
                    };
                    list.Add(addInput);
                }
            }

            //为0的话意味着只更改了状态，没修改到代理商，不为0意味着勾选上了其他代理商
            if (list.Count != 0)
            {
                await _agencySaleTicketClassAppService.AddAgencySaleTicketClassListAsync(list);
            }
            
            //2. 更新模板
            await _agencySaleTicketClassTemplateRepository.UpdateAsync(input.Id, m => Task.FromResult(agencySaleTicketClassTemplateUpdateInput4Save.MapTo(m)));

            return Result.Ok();
        }

        /// <summary>
        /// 根据条件获取代理商促销门票
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetAgencySaleTicketClassTemplateAsync<TDto>(IQuery<AgencySaleTicketClassTemplate> query)
        {
            return await _agencySaleTicketClassTemplateRepository.AsNoTracking().FirstOrDefaultAsync<AgencySaleTicketClassTemplate, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取代理商促销门票列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetAgencySaleTicketClassTemplateListAsync<TDto>(IQuery<AgencySaleTicketClassTemplate> query)
        {
            return await _agencySaleTicketClassTemplateRepository.AsNoTracking().ToListAsync<AgencySaleTicketClassTemplate, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取代理商促销门票下拉列表
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<DropdownDto> GetPermissionAgencySaleTicketClassTemplateDropdownAsync(Expression<Func<AgencySaleTicketClassTemplate, bool>> exp)
        {
            var parkIds = AbpSession.Parks;
            exp = exp.And(m => parkIds.Contains(m.ParkId));
            return await _agencySaleTicketClassTemplateRepository.AsNoTracking().Where(exp)
                .OrderBy(o => o.Id)
                .Select(m => new DropdownItem() { Text = m.AgencySaleTicketClassTemplateName, Value = m.Id })
                .ToDropdownDtoAsync();
        }

        /// <summary>
        /// 获取代理商促销票类模板列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public Task<PageResult<TDto>> GetPagedDataAsync<TDto>(IPageQuery<AgencySaleTicketClassTemplate> query = null)
        {
            return _agencySaleTicketClassTemplateRepository.AsNoTracking().ToPageResultAsync<AgencySaleTicketClassTemplate, TDto>(query);
        }

        #endregion
    }
}
