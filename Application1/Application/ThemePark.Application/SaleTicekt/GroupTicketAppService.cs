using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Core.ParkSale;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Infrastructure.Application;
using Abp.AutoMapper;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Infrastructure.EntityFramework;
using System.Data.Entity;
using Abp.Configuration;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.ParkSale.DomainServiceInterfaces;
using ThemePark.Infrastructure.Core;
using ThemePark.Core.BasicTicketType;
using ThemePark.Application.DataSync.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.EntityFramework.Uow;
using ThemePark.Application.DataSync.Interfaces;
using Newtonsoft.Json;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.DataSync;
using ThemePark.Core.InPark;
using ThemePark.Core.Settings;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Infrastructure.Enumeration;
using ThemePark.Core;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 团队票类服务
    /// </summary>
    public class GroupTicketAppService : ThemeParkAppServiceBase, IGroupTicketAppService
    {
        #region Fields
        private readonly IRepository<GroupTicket, string> _groupTicketRepository;
        private readonly IAgencySaleTicketClassAppService _agencySaleTicketClassAppService;
        private readonly IParkSaleTicketClassAppService _parkSaleTicketClassAppService;
        private readonly IInvoiceDomainService _invoiceDomainService;
        private readonly IUniqueCode _uniqueCode;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<TOBody, string> _toBodyRepository;
        private readonly ITOBodyDomainService _toBodyDomainService;
        private readonly IRepository<TicketInPark, long> _ticketInParkRepository;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IRepository<DefaultPrintSet> _defaultPrintSetRepository;
        private readonly IRepository<AgencyPrintSet> _agencyPrintSetRepository;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IRepository<TOHeader, string> _toHeaderRepository;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;
        private readonly ITradeInfoAppService _tradeInfoAppService;
        #endregion

        #region Cotr

        /// <summary>
        /// </summary>
        /// <param name="groupTicketRepository"></param>
        /// <param name="agencySaleTicketClassAppService"></param>
        /// <param name="parkSaleTicketClassAppService"></param>
        /// <param name="invoiceDomainService"></param>
        /// <param name="uniqueCode"></param>
        /// <param name="toHeaderRepository"></param>
        /// <param name="settingManager"></param>
        /// <param name="toBodyRepository"></param>
        /// <param name="toBodyDomainService"></param>
        /// <param name="ticketInParkRepository"></param>
        /// <param name="parkSaleTicketClassRepository"></param>
        /// <param name="invoiceAppService"></param>
        /// <param name="defaultPrintSetRepository"></param>
        /// <param name="agencyPrintSetRepository"></param>
        /// <param name="dataSyncManager"></param>
        /// <param name="invoiceCodeRepository"></param>
        /// <param name="tradeInfoAppService"></param>
        public GroupTicketAppService(IRepository<GroupTicket, string> groupTicketRepository, IAgencySaleTicketClassAppService agencySaleTicketClassAppService,
            IParkSaleTicketClassAppService parkSaleTicketClassAppService, IInvoiceDomainService invoiceDomainService, IUniqueCode uniqueCode,
            ISettingManager settingManager, IRepository<TOBody, string> toBodyRepository, ITOBodyDomainService toBodyDomainService,
            IRepository<TicketInPark, long> ticketInParkRepository, IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository,
            IInvoiceAppService invoiceAppService, IRepository<DefaultPrintSet> defaultPrintSetRepository, IRepository<AgencyPrintSet> agencyPrintSetRepository,
            IDataSyncManager dataSyncManager, IRepository<TOHeader, string> toHeaderRepository, IRepository<InvoiceCode> invoiceCodeRepository,
            ITradeInfoAppService tradeInfoAppService)
        {
            _groupTicketRepository = groupTicketRepository;
            _agencySaleTicketClassAppService = agencySaleTicketClassAppService;
            _parkSaleTicketClassAppService = parkSaleTicketClassAppService;
            _invoiceDomainService = invoiceDomainService;
            _uniqueCode = uniqueCode;
            _settingManager = settingManager;
            _toBodyRepository = toBodyRepository;
            _toBodyDomainService = toBodyDomainService;
            _ticketInParkRepository = ticketInParkRepository;
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _invoiceAppService = invoiceAppService;
            _defaultPrintSetRepository = defaultPrintSetRepository;
            _agencyPrintSetRepository = agencyPrintSetRepository;
            _dataSyncManager = dataSyncManager;
            _toHeaderRepository = toHeaderRepository;
            _invoiceCodeRepository = invoiceCodeRepository;
            _tradeInfoAppService = tradeInfoAppService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 生成票记录
        /// </summary>
        /// <param name="saleGroupTicketDto"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddGroupTicketAndReturnTradeNumAsync(SaleGroupTicketDto saleGroupTicketDto, int terminalId, int parkId)
        {
            //重新组装列表数据
            var separateTicketInput = SeparateTicket(saleGroupTicketDto.TicketInfos);

            //业务规则验证
            var result = await CheckGroupTicketAsync(saleGroupTicketDto, separateTicketInput.Count);
            if (!result.Success)
                return Result.FromError<string>(result.Message);

            //2. 支付
            var tradenoResult = await _tradeInfoAppService.AddTradeInfoAndReturnTradeInfoIdAsyn(saleGroupTicketDto.TradeInfos, parkId, saleGroupTicketDto.AgencyId);
            if (!tradenoResult.Success)
                return tradenoResult;

            //3. 生成票记录
            var input = new AddGroupTicketInput()
            {
                GroupInputs = saleGroupTicketDto.TicketInfos,
                Tradeno = tradenoResult.Data,
                InvoiceInput = saleGroupTicketDto.InvoiceInfos,
                AgencyId = saleGroupTicketDto.AgencyId,
                AgencyTypeId = saleGroupTicketDto.AgencyTypeId,
                OrderId = string.IsNullOrWhiteSpace(saleGroupTicketDto.OrderId) ? null : saleGroupTicketDto.OrderId,
            };

            var addResult = await AddGroupTicketAsync(input, parkId, AbpSession.TerminalId, separateTicketInput);
            if (!addResult.Success)
                return Result.FromError<string>(addResult.Message); ;

            return Result.FromData(tradenoResult.Data);
        }

        public async Task<GroupTicket> GetTicketById(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await _groupTicketRepository.FirstOrDefaultAsync(o => o.Id == id);
        }

        /// <summary>
        /// 更新团队售票实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateGroupTicketAsync(string id, GroupTicketUpdateForNewInput input)
        {
            var entity = await _agencySaleTicketClassAppService.GetByIdAsync(input.AgencySaleTicketClassId);

            input.Total = entity.SalePrice * input.Qty;

            await _groupTicketRepository.UpdateAsync(id, m => Task.FromResult(input.MapTo(m)));

            return Result.Ok();
        }

        /// <summary>
        /// 根据查询条件获取团体票信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetGroupTicketAsync<TDto>(IQuery<GroupTicket> query)
        {
            return await _groupTicketRepository.AsNoTracking().FirstOrDefaultAsync<GroupTicket, TDto>(query);
        }

        /// <summary>
        /// 根据条码获取团体票信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;GroupTicket&gt;.</returns>
        public async Task<GroupTicket> GetGroupTicketByBarcodeAsync(string barcode)
        {
            if (!await _groupTicketRepository.GetAll().AnyAsync(o => o.Id == barcode))
            {
                return null;
            }

            return await _groupTicketRepository.AsNoTrackingAndInclude(o => o.TicketInParks, o => o.GroupType, o => o.Invoice, o => o.Agency).FirstOrDefaultAsync(o => o.Id == barcode);
        }

        /// <summary>
        /// 根据条码确定门票是否未使用
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CheckTicketUnusedAsync(string barcode)
        {
            var date = DateTime.Today;

            return _groupTicketRepository.GetAll().AnyAsync(o => o.Id == barcode && o.TicketSaleStatus == TicketSaleStatus.Valid
                && o.InparkCounts == 0 && DbFunctions.DiffDays(o.ValidStartDate, date) >= 0 && DbFunctions.AddDays(o.ValidStartDate, o.ValidDays) >= date);
        }

        /// <summary>
        /// 手动入园
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> ManualInPark(string barcode)
        {
            //change state
            var ticket = await _groupTicketRepository.GetAsync(barcode);

            if (ticket.ParkId != AbpSession.LocalParkId)
            {
                return Result.FromCode(ResultCode.Fail, "套票不支持手动入园。");
            }

            var ticketClass = await _parkSaleTicketClassRepository.GetAllIncluding(o => o.TicketClass)
                .Where(o => o.Id == ticket.ParkSaleTicketClassId)
                .Select(o => o.TicketClass).SingleAsync();

            if (ticketClass.TicketClassMode != TicketClassMode.Normal)
            {
                return Result.FromCode(ResultCode.Fail, "套票不支持手动入园。");
            }

            var persons = ticketClass.TicketType.Persons * ticket.Qty;
            string remark = "手动入园";
            //add inpark
            _ticketInParkRepository.Insert(new TicketInPark()
            {
                Barcode = barcode,
                ParkId = AbpSession.LocalParkId,
                TerminalId = AbpSession.TerminalId,
                TicketClassId = ticketClass.Id,
                Qty = persons,
                Remark = remark
            });

            ticket.Remark = remark;
            ticket.TicketSaleStatus = TicketSaleStatus.InPark;
            ticket.InparkCounts = persons;

            //清掉验票的缓存
            var ticketCheckCacheDto = new TicketCheckCacheDto
            {
                Key = barcode
            };
            DataSyncInput dataSyncInput = new DataSyncInput()
            {
                SyncData = JsonConvert.SerializeObject(ticketCheckCacheDto),
                SyncType = DataSyncType.TicketCheckCacheClear
            };
            _dataSyncManager.UploadDataToTargetPark(ticket.ParkId, dataSyncInput);

            return Result.Ok();
        }

        /// <summary>
        /// 根据查询条件获取团体票信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetGroupTicketListAsync<TDto>(IQuery<GroupTicket> query)
        {
            return await _groupTicketRepository.AsNoTracking().ToListAsync<GroupTicket, TDto>(query);
        }

        /// <summary>
        /// 获取团体票默认有效期
        /// </summary>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        public async Task<Result<int>> GetGroupValidAsync()
        {
            var value = await _settingManager.GetSettingValueAsync(TicketSetting.GroupValid);
            return Result.FromData(int.Parse(value));
        }

        /// <summary>
        /// 获取代理商的有效日期
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetAgencyValidDays(int agencyTypeId, int agencyId)
        {
            var defaultResult = (await _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == agencyTypeId)) ?? new DefaultPrintSet();
            var validDays = defaultResult.DefaultValidDays;

            //获取代理商单独配置项
            var groupResult = _agencyPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyId == agencyId).Result;
            if (groupResult?.ValidDays == null)
                return validDays;
            //如果有团体配置，覆盖默认的打印价格类型和有效期配置
            validDays = groupResult.ValidDays.Value;
            return validDays;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// 验证支付数据
        /// </summary>
        /// <returns></returns>
        private async Task<Result> CheckGroupTicketAsync(ISaleTicketCheck<GroupTicketInput> input, int ticketCount)
        {
            //验证支付金额
            var amountResult = await CheckAmount(input);
            if (!amountResult.Success)
                return amountResult;

            //验证入园日期
            var timeResult = CheckInparkTime(input);
            if (!timeResult.Success)
                return timeResult;

            //验证发票号重复
            var invoiceResult = CheckInvoice(input, ticketCount);
            if (!invoiceResult.Success)
                return invoiceResult;

            var orderResult = await CheckOrder(input);
            if (!orderResult.Success)
                return orderResult;

            return Result.Ok();
        }

        /// <summary>
        /// 添加团队售票实体
        /// </summary>
        /// <returns></returns>
        private async Task<Result> AddGroupTicketAsync(AddGroupTicketInput addGroupTicketInput, int parkId, int terminalId, List<GroupTicketInput> separateTicketInput)
        {
            //传入的invoiceInput.InvoceNo是有效可用的，即数据库最大数已经加上1了
            var invoiceNo = long.Parse(addGroupTicketInput.InvoiceInput.InvoiceNo);

            var invoiceCode = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == addGroupTicketInput.InvoiceInput.InvoiceCode);

            var userId = AbpSession.GetUserId();

            //所有团体票
            List<GroupTicket> groupTickets = new List<GroupTicket>();
            //套票
            List<GroupTicket> mulTickets = new List<GroupTicket>();
            foreach (var item in separateTicketInput)
            {
                var entity = item.MapTo<GroupTicket>();
                var agencySaleTicket = await _agencySaleTicketClassAppService.GetByIdAsync(item.AgencySaleTicketClassId);
                entity.Id = await _uniqueCode.CreateAsync(CodeType.Barcode, parkId, terminalId);
                entity.Amount = item.SalePrice * item.Qty;
                entity.Price = agencySaleTicket.Price;
                entity.TradeInfoId = addGroupTicketInput.Tradeno;
                entity.TicketSaleStatus = TicketSaleStatus.Valid;
                entity.GroupTypeId = agencySaleTicket.GroupTypeId;
                entity.ValidStartDate = item.ValidStartDate.Date;
                //查询有效期
                entity.ValidDays = await GetAgencyValidDays(addGroupTicketInput.AgencyTypeId, addGroupTicketInput.AgencyId);
                entity.AgencyId = item.AgencyId;
                entity.ParkId = parkId;
                entity.TerminalId = terminalId;
                entity.CreatorUserId = userId;
                entity.ParkSaleTicketClassId = agencySaleTicket.ParkSaleTicketClassId;
                //旅行社取票传入订单Id
                entity.TOHeaderId = addGroupTicketInput.OrderId;
                var result = await _invoiceDomainService.AddInvoiceAndReturnIdAsync(new Invoice()
                {
                    Barcode = entity.Id,
                    InvoiceNo = invoiceNo.ToString(new string('0', addGroupTicketInput.InvoiceInput.InvoiceNo.Length)),
                    InvoiceCode = addGroupTicketInput.InvoiceInput.InvoiceCode,
                    IsActive = true,
                    TerminalId = terminalId
                });
                if (!result.Success)
                    return result;
                //更新Toheader交易号
                if (addGroupTicketInput.OrderId != null)
                    await _toHeaderRepository.UpdateAsync(addGroupTicketInput.OrderId,
                        p => Task.FromResult(p.TradeInfoId = addGroupTicketInput.Tradeno));
                //生成发票记录
                entity.InvoiceId = result.Data;

                groupTickets.Add(entity);

                if (item.TicketClassMode == TicketClassMode.MultiParkTicket)
                    mulTickets.Add(entity);

                //发票号递增
                if (invoiceCode.InvoiceNumIsIncrease)
                    invoiceNo = invoiceNo + 1;
                else
                    //发票号递减
                    invoiceNo = invoiceNo - 1;
            }

            //批量插入数据库
            await UnitOfWorkManager.Current.GetDbContext<ThemeParkDbContext>().BulkInsertAsync(groupTickets);
            
            //更新子订单状态为已消费
            if (!string.IsNullOrEmpty(addGroupTicketInput.OrderId))
            {
                var tobodies = await _toBodyRepository.GetAllListAsync(p => p.TOHeaderId == addGroupTicketInput.OrderId);
                tobodies.ForEach(p => _toBodyDomainService.ConsumedTOBody(p.Id));
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            IocManager.Instance.Resolve<IFixedRedisCache>().InvalidateSets(new[] { nameof(GroupTicket) });

            //套票同步
            foreach (var entity in mulTickets)
            {
                await MultiTicketDataSync(entity, parkId, terminalId);
            }

            //旅行社订单消费同步
            if (groupTickets.Any(m => m.TOHeaderId != null))
            {
                var toheaderId = groupTickets.Select(m => m.TOHeaderId).First();
                OrderConsume(new OrderConsumeDto() { TOHeaderId = toheaderId });
            }
            //foreach (var groupTicket in groupTickets)
            //{
            //    if (groupTicket.TOHeaderId != null)
            //    {
            //        OrderConsume(new OrderConsumeDto() { TOHeaderId = groupTicket.TOHeaderId });
            //    }
            //}

            return Result.Ok();
        }

        /// <summary>
        /// 验证旅行社订单是否存在
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Result> CheckOrder(ISaleTicketCheck<GroupTicketInput> input)
        {
            //订单号为空，为团体售票
            if (string.IsNullOrEmpty(input.OrderId))
                return Result.Ok();
            //旅行社订单、子订单状态为待付款
            var order = await _toHeaderRepository.FirstOrDefaultAsync(p => p.Id == input.OrderId && p.OrderType == OrderType.TravelOrder && p.TOBodies.All(o => o.OrderState == OrderState.WaitPay));
            if (order == null)
                return Result.FromCode(ResultCode.Fail, "订单状态异常无法取票");
            return Result.Ok();
        }

        /// <summary>
        /// 验证金额
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Result> CheckAmount(ISaleTicketCheck<GroupTicketInput> input)
        {
            //验证支付金额
            decimal amount = input.TradeInfos.Amount;

            //验证各个支付方式支付金额之和是否等于支付总金额
            decimal payDetailSum = input.TradeInfos.TradeInfoDetails.Sum(tradeInfodetail => tradeInfodetail.Amount);
            if (payDetailSum != amount)
                return Result.FromCode<decimal>(ResultCode.InvalidAmount);

            //后台重新计算交易总金额
            input.TradeInfos.Amount = amount;

            return Result.Ok();
        }

        /// <summary>
        /// 验证入园日期
        /// </summary>
        /// <returns></returns>
        private Result CheckInparkTime(ISaleTicketCheck<GroupTicketInput> input)
        {
            if (input.TicketInfos.Any(ticket => ticket.ValidStartDate.Date < DateTime.Now.Date))
            {
                return Result.FromError("入园日期不能早于今天，请重新选择", ResultCode.InvalidData);
            }
            return Result.Ok();
        }

        /// <summary>
        /// 验证入园日期
        /// </summary>
        /// <returns></returns>
        private Result CheckInvoice(ISaleTicketCheck<GroupTicketInput> input, int ticketCount)
        {
            var invoiceCode = _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == input.InvoiceInfos.InvoiceCode).Result;
            if (null == invoiceCode)
                return Result.FromError("发票代码不存在");

            var existed = _invoiceAppService.CheckIfExisteInValidOrDuplicateInvoice(input.InvoiceInfos.InvoiceCode, input.InvoiceInfos.InvoiceNo, ticketCount, invoiceCode.InvoiceNumIsIncrease);
            if (existed)
                return Result.FromCode<List<PrintInfo>>(ResultCode.DuplicateInvoiceRecord);

            return Result.Ok();
        }

        /// <summary>
        /// 同步套票数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        private async Task MultiTicketDataSync(GroupTicket entity, int parkId, int terminalId)
        {
            //同步套票数据
            var dto = new MultiTicketSendDto
            {
                TicketClassType = TicketCategory.GroupTicket,
                Barcode = entity.Id,
                Amount = entity.Amount,
                InparkCounts = 0,
                FromParkid = parkId,
                ParkSaleTicketClassId = entity.ParkSaleTicketClassId,
                Price = entity.Price,
                Qty = entity.Qty,
                SalePrice = entity.SalePrice,
                TerminalId = terminalId,
                ValidDays = entity.ValidDays,
                ValidStartDate = entity.ValidStartDate,
                AgencySaleTicketClassId = entity.AgencySaleTicketClassId,
                TradeInfoId = entity.TradeInfoId,
                AgencyId = entity.AgencyId,
                GroupTypeId = entity.GroupTypeId,

                CreatorUserId = AbpSession.UserId,
                CreationTime = entity.CreationTime,
                InvoiceId = entity.InvoiceId,
                LastModificationTime = entity.LastModificationTime,
                LastModifierUserId = entity.LastModifierUserId,
                TOBodyId = entity.TOBodyId,
                TOHeaderId = entity.TOHeaderId,
                SyncTicketType = entity.SyncTicketType

            };
            var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
            var syncInput = new DataSyncInput
            {
                SyncType = DataSyncType.MultiTicketSend,
                SyncData = JsonConvert.SerializeObject(dto)
            };
            var parkSaleTicketClass = await _parkSaleTicketClassRepository.FirstOrDefaultAsync(p => p.Id == entity.ParkSaleTicketClassId);
            var otherParkIds = parkSaleTicketClass?.TicketClass.InParkIdFilter.Split(',');
            //var result = await _parkSaleTicketClassAppService.GetMulSaleTicketByParkIdsAsync<ParkSaleTicketClassCacheItem>(new List<int>() { parkId }, null);
            //var otherParkIds = result.Where(p => p.Id == entity.ParkSaleTicketClassId).FirstOrDefault().InParkIdFilter.Trim().Split(',');
            foreach (var parkid in otherParkIds)
            {
                if (parkid != parkId.ToString())
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                }
            }
        }

        /// <summary>
        /// 同步旅行社取票数据到中心（避免中心可取消订单）
        /// </summary>
        /// <returns></returns>
        private void OrderConsume(OrderConsumeDto data)
        {
            var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
            DataSyncInput input = new DataSyncInput()
            {
                SyncData = JsonConvert.SerializeObject(data),
                SyncType = DataSyncType.OrderConsume
            };
            dataSyncManager.UploadDataToTargetPark(ThemeParkConsts.CenterParkId, input);
        }



        /// <summary>
        /// 拆分购票数据
        /// </summary>
        /// <param name="intput"></param>
        /// <returns></returns>
        private List<GroupTicketInput> SeparateTicket(List<GroupTicketInput> intput)
        {
            List<GroupTicketInput> printTicket = new List<GroupTicketInput>();


            foreach (var saleTicket in intput)
            {
                //票总数
                int qty = saleTicket.Qty;
                //多少张票合并一张
                int numPerTicket = saleTicket.IsAllOutPut;
                for (int i = 0; i < qty / numPerTicket; i++)
                {
                    var ticket = new GroupTicketInput()
                    {
                        Qty = numPerTicket,
                        IsAllOutPut = numPerTicket,
                        AgencyId = saleTicket.AgencyId,
                        AgencySaleTicketClassId = saleTicket.AgencySaleTicketClassId,
                        ValidStartDate = saleTicket.ValidStartDate,
                        TicketClassMode = saleTicket.TicketClassMode,
                        SalePrice = saleTicket.SalePrice,
                        TOBodyId = saleTicket.TOBodyId
                    };

                    printTicket.Add(ticket);
                }
                if (qty % numPerTicket != 0)
                {
                    var ticket = new GroupTicketInput()
                    {
                        Qty = qty % numPerTicket,
                        IsAllOutPut = qty % numPerTicket,
                        AgencyId = saleTicket.AgencyId,
                        AgencySaleTicketClassId = saleTicket.AgencySaleTicketClassId,
                        ValidStartDate = saleTicket.ValidStartDate,
                        TicketClassMode = saleTicket.TicketClassMode,
                        SalePrice = saleTicket.SalePrice,
                        TOBodyId = saleTicket.TOBodyId
                    };

                    printTicket.Add(ticket);
                }
            }
            return printTicket;
        }

        #endregion

    }
}
