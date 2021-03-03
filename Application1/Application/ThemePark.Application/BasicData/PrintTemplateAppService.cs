using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using GroupType = ThemePark.Application.BasicData.Dto.GroupType;

namespace ThemePark.Application.BasicData
{
    public class PrintTemplateAppService : ThemeParkAppServiceBase, IPrintTemplateAppService
    {
        private readonly IRepository<PrintTemplate> _printTemplateRepository;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;
        private readonly IRepository<PrintTemplateDetail> _printTemplateDetailRepository;
        private readonly IRepository<TicketPrintSet> _ticketPrintSetRepository;
        private readonly IRepository<DefaultPrintSet> _defaultPrintSetRepository;
        private readonly IRepository<AgencyType> _agencyTypeRepository;
        private readonly IRepository<TicketClass> _ticketClassRepository;
        private readonly IRepository<ParkAgency> _parkAgencyRepository;
        private readonly IRepository<AgencyPrintSet> _agencyPrintSetRepository;
        private readonly ISettingManager _settingManager;

        public PrintTemplateAppService(IRepository<PrintTemplate> printTemplateRepository, IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository, IRepository<PrintTemplateDetail> printTemplateDetailRepository, IRepository<TicketPrintSet> ticketPrintSetRepository, IRepository<DefaultPrintSet> defaultPrintSetRepository, IRepository<AgencyType> agencyTypeRepository, ISettingManager settingManager, IRepository<TicketClass> ticketClassRepository, IRepository<ParkAgency> parkAgencyRepository, IRepository<AgencyPrintSet> agencyPrintSetRepository)
        {
            _printTemplateRepository = printTemplateRepository;
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _printTemplateDetailRepository = printTemplateDetailRepository;
            _ticketPrintSetRepository = ticketPrintSetRepository;
            _defaultPrintSetRepository = defaultPrintSetRepository;
            _agencyTypeRepository = agencyTypeRepository;
            _settingManager = settingManager;
            _ticketClassRepository = ticketClassRepository;
            _parkAgencyRepository = parkAgencyRepository;
            _agencyPrintSetRepository = agencyPrintSetRepository;
        }


        /// <summary>
        /// 新增打印模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<int>> AddPrintTemplateAsync(AddPrintTemplateInput input)
        {
            //验证是否有重复记录
            var check = await _printTemplateRepository.GetAll().AnyAsync(p => p.TemplateName == input.TemplateName);
            if (check)
                return Result.FromCode<int>(ResultCode.DuplicateRecord);
            var Id = await _printTemplateRepository.InsertAndGetIdAsync(input.MapTo<PrintTemplate>());
            return Result.FromData(Id);
        }


        /// <summary>
        /// 修改打印模板
        /// </summary>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> UpdatePrintTemplateAsync(UpdatePrintTemplateInput input, int id)
        {
            //验证是否有重复记录
            var check = await _printTemplateRepository.GetAll().AnyAsync(p => p.TemplateName == input.TemplateName && p.Id != id);
            if (check)
                return Result.FromCode(ResultCode.DuplicateRecord);
            await _printTemplateRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));

            return Result.FromData(id);
        }

        /// <summary>
        /// 获取所有打印模板名称
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetAllFileNamesDto>>> GetAllFileNameAsync()
        {
            var data = (await _printTemplateRepository.GetAllListAsync()).MapTo<List<GetAllFileNamesDto>>();
            return Result.FromData(data);
        }

        /// <summary>
        /// 获取打印模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public async Task<Result<GetTemplateDto>> GetPrintTemplate(string templateName)
        {
            var data = await _printTemplateRepository.FirstOrDefaultAsync(p => p.TemplateName == templateName);
            return Result.FromData(data.MapTo<GetTemplateDto>());
        }

        /// <summary>
        /// 获取促销票、年卡、入园单、补票
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetPrintTicketDto>>> GetPrintTicket(int parkid, bool isContainUsed)
        {
            var today = DateTime.Today;
            //取促销票\年卡
            var list = (await _parkSaleTicketClassRepository.GetAllListAsync(p => p.TicketClassStatus == TicketClassStatus.Sailing && p.ParkId == parkid && (today <= p.SaleEndDate.Value || p.SaleStartDate == null))).TODtos();

            //去除已配置打印模板的票类
            if (!isContainUsed)
            {
                var ids = list.Select(p => p.Id).ToList();
                await IsTicketTypeIdInvalid(ids, false);
                list = list.Where(p => ids.Contains(p.Id)).ToList();
            }
            return Result.FromData(list);

        }

        /// <summary>
        /// 获取年卡
        /// </summary>
        /// <param name="parkid"></param>
        /// <returns></returns>
        public async Task<Result<List<GetPrintTicketDto>>> GetPrintYearCard(int parkid)
        {
            //取年卡\多园年卡
            var list = (await _parkSaleTicketClassRepository.GetAllListAsync(p => p.TicketClassStatus == TicketClassStatus.Sailing && p.ParkId == parkid && (p.TicketClass.TicketClassMode == TicketClassMode.YearCard || p.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard))).MapTo<List<GetPrintTicketDto>>();

            //去除已配置打印模板的票类
            var ids = list.Select(p => p.Id).ToList();
            await IsTicketTypeIdInvalid(ids, true);
            list = list.Where(p => ids.Contains(p.Id)).ToList();
            return Result.FromData(list);
        }

        /// <summary>
        /// 新增打印模板配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddPrintTemplateConfig(AddConfigInput input)
        {
            if (!input.IsYearCard && !await IsTicketTypeIdInvalid(input.ParkSaleTicketClassIds, false))
                return Result.FromCode(ResultCode.DuplicateRecord);
            if (input.IsYearCard && !await IsTicketTypeIdInvalid(input.ParkSaleTicketClassIds, true))
                return Result.FromCode(ResultCode.DuplicateRecord);

            foreach (var id in input.ParkSaleTicketClassIds)
            {
                var entity = new PrintTemplateDetail()
                {
                    PrintTemplateId = input.PrintTemplateId,
                    TicketClassId = id,
                    Type = input.IsYearCard ? PrintTemplateType.YearCard : DeterminePrintTicketType(id)
                };
                await _printTemplateDetailRepository.InsertAndGetIdAsync(entity);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 查询打印模板配置
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<SearchPrintConfigDto>>> SearchConfig(SearchPrintConfigInput input)
        {
            List<SearchPrintConfigDto> dtos;

            //根据是否传模板ID票类ID拼接查询条件
            Expression<Func<PrintTemplateDetail, bool>> query = p => true;
            if (input.PrintTemplateId.HasValue)
                query = query.And(p => p.PrintTemplateId == input.PrintTemplateId);

            //拼凑数据
            dtos = await _printTemplateDetailRepository.GetAll().ToListAsync<PrintTemplateDetail, SearchPrintConfigDto>(new Query<PrintTemplateDetail>(query));
            if (input.TicketClassId.HasValue)
            {
                var config = dtos.Where(p => p.TicketClassId == input.TicketClassId).Select(p => p.PrintTemplateId).Distinct();
                dtos.RemoveAll(p => !config.Contains(p.PrintTemplateId));
            }
            var today = DateTime.Today;
            foreach (var dto in dtos)
            {
                switch (dto.TicketClassId)
                {
                    case PrintTemplateSetting.ExcessFareTicketClasId:
                        dto.ParkSaleTicketClassName = PrintTemplateSetting.ExcessFareTicketName;
                        break;
                    case PrintTemplateSetting.InParkBillTicketClassId:
                        dto.ParkSaleTicketClassName = PrintTemplateSetting.InParkBillName;
                        break;
                    default:
                        var parkSaleTicketClass = await _parkSaleTicketClassRepository.FirstOrDefaultAsync
                            (p => p.Id == dto.TicketClassId
                            && p.TicketClassStatus == TicketClassStatus.Sailing
                            && (p.SaleStartDate == null || p.SaleEndDate == null || today <= p.SaleEndDate.Value));
                        if (parkSaleTicketClass == null)
                            continue;
                        dto.ParkSaleTicketClassName = parkSaleTicketClass.SaleTicketClassName;
                        //年卡模板票类加-年卡后缀
                        if (dto.Type == PrintTemplateType.YearCard)
                            dto.ParkSaleTicketClassName += "(年卡)";
                        break;
                }
            }
            //票类已删除或下架，从返回数据里去除
            dtos.RemoveAll(p => p.ParkSaleTicketClassName == null);

            return Result.FromData(dtos);
        }


        /// <summary>
        /// 查询代理商打印价格设置
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result<List<SearchAgencyPriceSetDto>>> SearchAgencyPriceSet(SearchAgencyPriceSetInput input, int parkId)
        {
            //根据是否传模板ID票类ID拼接查询条件
            Expression<Func<ParkAgency, bool>> query = p => p.ParkId == parkId;

            if (!string.IsNullOrEmpty(input.AgencyName))
                query = query.And(p => p.Agency.AgencyName.Contains(input.AgencyName));

            List<SearchAgencyPriceSetDto> dtos = new List<SearchAgencyPriceSetDto>();
            var parkAgencyResults = await _parkAgencyRepository.GetAllListAsync(query);
            foreach (var parkAgency in parkAgencyResults)
            {
                var agencyPrintSetResult = await _agencyPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyId == parkAgency.AgencyId && p.AgencyTypeId == parkAgency.AgencyTypeId);
                var dto = agencyPrintSetResult.MapTo<SearchAgencyPriceSetDto>() ?? new SearchAgencyPriceSetDto()
                {
                    AgencyId = parkAgency.AgencyId,
                    AgencyTypeId = parkAgency.AgencyTypeId,
                };
                dto.AgencyName = parkAgency?.Agency?.AgencyName;
                dto.AgencyTypeName = parkAgency?.AgencyType?.AgencyTypeName;
                dtos.Add(dto);
            }

            return Result.FromData(dtos);
        }


        /// <summary>
        /// 保存代理商打印价格设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> SaveOrInsertAgencyPricePrintSet(AgencyPricePrintSetInput input)
        {
            foreach (var config in input.AgencyInfos)
            {
                //数据库是否有记录
                var isCongifExit = await _agencyPrintSetRepository.GetAll().AnyAsync(p => p.AgencyId == config.AgencyId);
                var entity = input.MapTo<AgencyPrintSet>();
                entity.AgencyId = config.AgencyId;
                entity.AgencyTypeId = config.AgencyTypeId;
                //新增数据，代理商没有配置过
                if (config.Id == 0 && !isCongifExit)
                    await _agencyPrintSetRepository.InsertAsync(entity);
                //新增数据，代理商已配置
                else if (config.Id == 0 && isCongifExit)
                    return Result.FromCode(ResultCode.DuplicateRecord);
                //修改数据
                else if (config.Id != 0)
                    await _agencyPrintSetRepository.UpdateAsync(config.Id, p => Task.FromResult(input.MapTo(p)));

            }
            return Result.Ok();
        }


        /// <summary>
        /// 查询代理商有效期设置
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result<List<SearchAgencyValidDaySetDto>>> SearchAgencyValidDaysSet(SearchAgencyValidDaysSetInput input, int parkId)
        {
            //根据是否传模板ID票类ID拼接查询条件
            Expression<Func<ParkAgency, bool>> query = p => p.ParkId == parkId;

            if (!string.IsNullOrEmpty(input.AgencyName))
                query = query.And(p => p.Agency.AgencyName.Contains(input.AgencyName));

            List<SearchAgencyValidDaySetDto> dtos = new List<SearchAgencyValidDaySetDto>();
            var parkAgencyResults = await _parkAgencyRepository.GetAllListAsync(query);
            foreach (var parkAgency in parkAgencyResults)
            {
                var agencyPrintSetResult = await _agencyPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyId == parkAgency.AgencyId && p.AgencyTypeId == parkAgency.AgencyTypeId);
                var dto = agencyPrintSetResult.MapTo<SearchAgencyValidDaySetDto>() ?? new SearchAgencyValidDaySetDto()
                {
                    AgencyId = parkAgency.AgencyId,
                    AgencyTypeId = parkAgency.AgencyTypeId,
                };
                dto.AgencyName = parkAgency?.Agency?.AgencyName;
                dto.AgencyTypeName = parkAgency.AgencyType.AgencyTypeName;
                dtos.Add(dto);
            }

            return Result.FromData(dtos);
        }


        /// <summary>
        /// 保存团体有效期设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<Result> SaveOrInsertAgencyValidDaysPrintSet(AgencyValidDaysSetInput input)
        {
            foreach (var config in input.AgencyInfos)
            {
                var isCongifExit = await _agencyPrintSetRepository.GetAll().AnyAsync(p => p.AgencyId == config.AgencyId);
                var entity = input.MapTo<AgencyPrintSet>();
                entity.AgencyId = config.AgencyId;
                entity.AgencyTypeId = config.AgencyTypeId;

                //新增数据，代理商没有配置过
                if (config.Id == 0 && !isCongifExit)
                    await _agencyPrintSetRepository.InsertAsync(entity);
                //新增数据，代理商已配置
                else if (config.Id == 0 && isCongifExit)
                    return Result.FromCode(ResultCode.DuplicateRecord);
                //修改数据
                else if (config.Id != 0)
                    await _agencyPrintSetRepository.UpdateAsync(config.Id, p => Task.FromResult(input.MapTo(p)));

            }
            return Result.Ok();
        }


        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Result> DeleteConfigs(List<int> ids)
        {
            await Task.Run(() => ids.ForEach(p => _printTemplateDetailRepository.DeleteAsync(p)));
            return Result.Ok();
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateConfig(UpdatePrintConfig input)
        {
            await _printTemplateDetailRepository.UpdateAsync(input.Id, p => Task.FromResult(input.MapTo(p)));
            return Result.Ok();
        }


        /// <summary>
        /// 获取所有基础票类打印参数配置
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetTicketPrintSetsDto>>> GetTicketPrintSets(int parkId)
        {
            var ticketClasses = await _ticketClassRepository.GetAllListAsync(p => p.ParkId == parkId);
            var result = new List<GetTicketPrintSetsDto>();

            foreach (var tickettype in ticketClasses)
            {
                var ticketPrintSet = await _ticketPrintSetRepository.FirstOrDefaultAsync(p => p.TicketClassId == tickettype.Id);
                var dto = ticketPrintSet.MapTo<GetTicketPrintSetsDto>() ?? new GetTicketPrintSetsDto();
                dto.TicketClassName = tickettype.TicketClassName;
                dto.TicketClassId = tickettype.Id;
                result.Add(dto);
            }
            return Result.FromData(result.MapTo<List<GetTicketPrintSetsDto>>());
        }


        /// <summary>
        /// 保存基础票类打印参数，如果没有记录则新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> SaveOrInsertTicketPrintSet(GetTicketPrintSetsDto input)
        {
            //业务验证 基础票类重复
            if (await _ticketPrintSetRepository.GetAll().AnyAsync(p => p.TicketClassId == input.TicketClassId && p.Id != input.Id))
                return Result.FromCode(ResultCode.DuplicateRecord);

            var entity = input.MapTo<TicketPrintSet>();
            //Id为0没有此条记录 新增
            if (entity.Id == 0)
                await _ticketPrintSetRepository.InsertAsync(entity);
            else
                await _ticketPrintSetRepository.UpdateAsync(entity.Id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        /// <summary>
        /// 获取所有打印价格设置
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetPricePrintSetsDto>>> GetPricePrintSets()
        {
            var results = new List<GetPricePrintSetsDto>();
            var groupPriceSet = await _settingManager.GetSettingValueAsync(TicketSetting.GroupPriceSet);

            results.Add(new GetPricePrintSetsDto()
            {
                GroupType = GroupType.Group,
                PrintPriceType = (PrintPriceType)int.Parse(groupPriceSet)
            });

            var nonGroupPriceSet = await _settingManager.GetSettingValueAsync(TicketSetting.NonGroupPriceSet);
            results.Add(new GetPricePrintSetsDto()
            {
                GroupType = GroupType.NonGroup,
                PrintPriceType = (PrintPriceType)int.Parse(nonGroupPriceSet)
            });

            return Result.FromData(results);

        }

        /// <summary>
        /// 保存价格设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> SavePricePrintSet(PricePrintSetInput input)
        {
            switch (input.GroupType)
            {
                case GroupType.Group:
                    await _settingManager.ChangeSettingForApplicationAsync(TicketSetting.GroupPriceSet, ((int)input.PrintPriceType).ToString());
                    return Result.Ok();
                case GroupType.NonGroup:
                    await _settingManager.ChangeSettingForApplicationAsync(TicketSetting.NonGroupPriceSet, ((int)input.PrintPriceType).ToString());
                    return Result.Ok();
                default:
                    return Result.FromCode(ResultCode.NoRecord);
            }
        }


        /// <summary>
        /// 默认有效期设置
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetValidDaysPrintSetsDto>>> GetValidDaysPrintSets()
        {
            var agencyTypes = await _agencyTypeRepository.GetAllListAsync();
            var configs = new List<GetValidDaysPrintSetsDto>();

            //以代理商类型为基准，假如没有配置数据则新建一个，保证返回的数据包含所有代理商
            foreach (var agencyType in agencyTypes)
            {
                var groupConfig = await _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == agencyType.Id);
                var groupDto = groupConfig.MapTo<GetValidDaysPrintSetsDto>() ?? new GetValidDaysPrintSetsDto();
                groupDto.AgencyTypeName = agencyType.AgencyTypeName;
                groupDto.AgencyTypeId = agencyType.Id;
                configs.Add(groupDto);
            }

            //散客有效期
            var nonGroupConfig = await _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == null);
            var nonGroupDto = nonGroupConfig.MapTo<GetValidDaysPrintSetsDto>() ?? new GetValidDaysPrintSetsDto();
            nonGroupDto.AgencyTypeName = "散客";

            configs.Add(nonGroupDto);
            return Result.FromData(configs);
        }


        /// <summary>
        /// 保存有效期打印设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> SaveOrInsertValidDaysPrintSet(ValidDaysPrintSetInput input)
        {
            //验证业务规则
            if (await _defaultPrintSetRepository.GetAll().AnyAsync(p => p.AgencyTypeId == input.AgencyTypeId && p.Id != input.Id))
                return Result.FromCode(ResultCode.DuplicateRecord);

            var entity = input.MapTo<DefaultPrintSet>();
            //ID不存在则新增记录 反之更新数据
            if (entity.Id == 0)
                await _defaultPrintSetRepository.InsertAsync(entity);
            else
                await _defaultPrintSetRepository.UpdateAsync(input.Id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();

        }


        /// <summary>
        /// 判断促销票类Id列是否有效
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsTicketTypeIdInvalid(List<int> ids, bool isYearCard)
        {
            var length = ids.Count;
            var quey = new Query<PrintTemplateDetail>(p => ids.Contains(p.TicketClassId)).GetFilter();
            quey = isYearCard ? quey.And(p => p.Type == PrintTemplateType.YearCard) : quey.And(p => p.Type != PrintTemplateType.YearCard);
            //查询已配置的id
            var list =
                (await _printTemplateDetailRepository.AsNoTracking()
                    .ToListAsync<PrintTemplateDetail, SearchPrintTemplateDetail>(
                        new Query<PrintTemplateDetail>(quey))).Select(p => p.TicketClassId);

            ids.RemoveAll(p => list.Contains(p));

            if (length != ids.Count)
                return false;

            return true;
        }


        /// <summary>
        /// 根据id返回对应打印模板类型
        /// </summary>
        /// <param name="printTicketClassId"></param>
        /// <returns></returns>
        private PrintTemplateType DeterminePrintTicketType(int printTicketClassId)
        {
            switch (printTicketClassId)
            {
                case PrintTemplateSetting.InParkBillTicketClassId:
                    return PrintTemplateType.InParkBill;
                case PrintTemplateSetting.ExcessFareTicketClasId:
                    return PrintTemplateType.ExcessFare;
                default:
                    {
                        return PrintTemplateType.Ticket;
                    }

            }

        }
    }
}
