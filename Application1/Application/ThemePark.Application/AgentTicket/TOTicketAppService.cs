using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Application.DataSync;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.SaleTicekt;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Common;
using ThemePark.Core;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.AgentTicket.Repositories;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CardManage;
using ThemePark.Core.CardManage.Repositories;
using ThemePark.Core.DataSync;
using ThemePark.Core.InPark;
using ThemePark.Core.ParkSale;
using ThemePark.Core.ParkSale.DomainServiceInterfaces;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Core;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Infrastructure.Enumeration;
using ThemePark.Infrastructure.Web.Api;

namespace ThemePark.Application.AgentTicket
{
    public class TOTicketAppService : ThemeParkAppServiceBase, ITOTicketAppService
    {
        #region Fields
        private readonly ITOTicketDomainService _toTicketDomainService;
        private readonly IRepository<TOBody, string> _toBodyRepository;
        private readonly IRepository<TOVoucher, string> _toVoucherRepository;
        private readonly IInvoiceDomainService _invoiceDomainService;
        private readonly ITOBodyDomainService _iTOBodyDomainService;
        private readonly IUniqueCode _uniqueCode;
        private readonly ITOTicketRepository _toTicketRepository;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketClassRepository;
        private readonly IRepository<TicketInPark, long> _ticketInParkRepository;
        private readonly IRepository<DefaultPrintSet> _defaultPrintSetRepository;
        private readonly IRepository<AgencyPrintSet> _agencyPrintSetRepository;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;
        private readonly IRepository<SyncPark> _syncParkRepositoty;
        private readonly IVipVoucherRepository _parkVipVoucherRepository;
        private readonly INonGroupTicketAppService _nonGroupTicketAppService;
        #endregion

        #region Ctor

        /// <summary>
        /// 依赖注入
        /// </summary>
        public TOTicketAppService(ITOTicketDomainService toTicketDomainService, IRepository<InvoiceCode> invoiceCodeRepository,
            IRepository<TOBody, string> toBodyRepository, IRepository<TOVoucher, string> toVoucherRepository, IInvoiceDomainService invoiceDomainService,
            ITOBodyDomainService iTOBodyDomainService, IUniqueCode uniqueCode, ITOTicketRepository tOTicketRepository,
            IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository, IDataSyncManager dataSyncManager,
            IRepository<AgencySaleTicketClass> agencySaleTicketClassRepository, IRepository<TicketInPark, long> ticketInParkRepository,
            IRepository<DefaultPrintSet> defaultPrintSetRepository, IRepository<AgencyPrintSet> agencyPrintSetRepository,
            IVipVoucherRepository parkVipVoucherRepository, INonGroupTicketAppService nonGroupTicketAppService,
            IRepository<SyncPark> syncParkRepositoty)
        {
            _toTicketDomainService = toTicketDomainService;
            _toBodyRepository = toBodyRepository;
            _toVoucherRepository = toVoucherRepository;
            _invoiceDomainService = invoiceDomainService;
            _iTOBodyDomainService = iTOBodyDomainService;
            _uniqueCode = uniqueCode;
            _toTicketRepository = tOTicketRepository;
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _dataSyncManager = dataSyncManager;
            _agencySaleTicketClassRepository = agencySaleTicketClassRepository;
            _ticketInParkRepository = ticketInParkRepository;
            _defaultPrintSetRepository = defaultPrintSetRepository;
            _agencyPrintSetRepository = agencyPrintSetRepository;
            _invoiceCodeRepository = invoiceCodeRepository;
            _parkVipVoucherRepository = parkVipVoucherRepository;
            _nonGroupTicketAppService = nonGroupTicketAppService;
            _syncParkRepositoty = syncParkRepositoty;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 网络票增加取票记录
        /// </summary>
        /// <param name="input"></param>
        /// <param name="terminalId"></param>
        public async Task<Result<List<string>>> AddWebTicketAsync(ToTicketInput input, int terminalId)
        {
            var barcodes = new List<string>();
            var mulTicket = new List<string>();
            var vipVouchers = new List<TOTicket>();

            long invoiceNo = 0;
            InvoiceCode invoiceCode = null;
            if (input.InvoiceInfos != null)
            {
                invoiceNo = long.Parse(input.InvoiceInfos.InvoiceNo);
                invoiceCode = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == input.InvoiceInfos.InvoiceCode);
            }

            foreach (var toBodyId in input.TOBodyIds)
            {
                //作废电子票
                var checkResult = await TakeWebTicketValidate(toBodyId);
                if (!checkResult.Success)
                    return Result.FromCode<List<string>>(checkResult.Code, checkResult.Message);
                var eTickets = checkResult.Data;
                var ticketList = new List<TOTicket>();

                foreach (var eTicket in eTickets)
                {
                    await _toTicketDomainService.InvalidTicket(eTicket.Id);

                    //创建门票 默认分票输出Qty = 1,InvoiceNo 后续补上
                    var entity = new TOTicket()
                    {
                        //parkid取取票公园ID
                        ParkId = input.FromParkId,
                        Qty = 1,
                        AgencySaleTicketClassId = eTicket.AgencySaleTicketClassId,
                        ValidStartDate = (eTicket.ValidStartDate < DateTime.Today ? DateTime.Today : eTicket.ValidStartDate),
                        ValidDays = GetValidDays(eTicket.AgencySaleTicketClass.AgencySaleTicketClassTemplate.AgencyTypeId, eTicket.AgencySaleTicketClass.AgencyId),
                        Price = eTicket.Price,
                        SalePrice = eTicket.SalePrice,
                        SettlementPrice = eTicket.SettlementPrice,
                        ParkSettlementPrice = eTicket.ParkSettlementPrice,
                        //分票输出 总价等于票价
                        Amount = eTicket.SalePrice,
                        TOVoucherId = eTicket.TOVoucherId,
                        TicketSaleStatus = TicketSaleStatus.Valid,
                        TerminalId = input.TerminalId,
                        CreatorUserId = AbpSession.UserId ?? input.CreatorUserId,
                        TicketFormEnum = TicketFormEnum.PaperTicket,
                        Invoice = input.InvoiceInfos == null ? null : new Invoice()
                        {
                            InvoiceNo = invoiceNo.ToString(new string('0', input.InvoiceInfos.InvoiceNo.Length)),
                            InvoiceCode = input.InvoiceInfos.InvoiceCode,
                            TerminalId = input.TerminalId,
                            IsActive = true
                        }
                    };
                    //parkId取实际生成票公园ID
                    await _toTicketDomainService.InitialTOTicketAsync(entity,input.ParkId,terminalId);
                    ticketList.Add(entity);
                    barcodes.Add(entity.Id);

                    //发票号递增
                    if (invoiceCode != null && invoiceCode.InvoiceNumIsIncrease)
                        invoiceNo = invoiceNo + 1;
                    else if (invoiceCode != null && !invoiceCode.InvoiceNumIsIncrease)
                        //发票号递减
                        invoiceNo = invoiceNo - 1;

                    //获取需要同步套票数据
                    if (eTicket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.MultiParkTicket)
                    {
                        mulTicket.Add(eTicket.TOVoucherId);
                    }

                    //年卡凭证取票
                    if (eTicket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard || eTicket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.YearCard)
                    {
                        vipVouchers.Add(entity);
                    }
                }

                //批量新增取票记录 BulkInsertAsync方法没有把关联表数据插入
                _toTicketRepository.AddRange(ticketList);

                //更新子订单状态
                await _iTOBodyDomainService.ConsumedTOBody(toBodyId);
                await UnitOfWorkManager.Current.SaveChangesAsync();

                //核销订单
                //todo: 是否需要每张票都核销一次？还是一张票取了就把整笔交易核销掉？
                OrderConsume(new OrderConsumeDto() { SubOrderid = toBodyId });
            }
        
            //年卡凭证生成
            await CreateVipCardVoucher(vipVouchers, terminalId);

            //套票同步数据
            foreach (var toVoucherId in mulTicket)
            {
                await MultiTicketDataSync(toVoucherId, input.TerminalId);
            }

            return Result.FromData(barcodes);
        }

        /// <summary>
        /// 查询网络订单
        /// </summary>
        /// <param name="pidOrTicketCode"></param>
        /// <returns></returns>
        public async Task<Result<List<GetWebTicketOrderDto>>> GetWebTicketOrderAsync(string pidOrTicketCode)
        {
            var plandate = DateTime.Now.Date;

            //查询条件
            //身份证等于pidOrTicketCode或主订单号等于pidOrTicketCode（订票返回的取票码实际是主订单号）
            //订单状态 WaitCost
            //入园日期 晚于当天或者有效截止日期晚于当天
            //订单类型属于OTA订单
            var result =
               await _iTOBodyDomainService.GetTOBodyListAsync<GetWebTicketOrderDto>(
                    new Query<TOBody>(p => (p.Customer.Pid == pidOrTicketCode || p.TOHeaderId.Contains(pidOrTicketCode)) && p.OrderState == OrderState.WaitCost
                    && (p.TOHeader.ValidStartDate >= plandate || p.TOHeader.ValidEndDate.Value >= plandate) && p.TOHeader.OrderType == OrderType.OTAOrder));

            if (result.Data == null)
                return result;
            var toHeaders = result.Data.OrderBy(p => p.ValidStartDate).ToList().Distinct(new IsTOHeaderEqual());
            return Result.FromData(toHeaders.ToList());
        }


        /// <summary>
        /// 旅行社取票
        /// </summary>
        /// <param name="input"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result<List<string>>> AddTravelTicketAsync(AddTravelTicketInput input, int terminalId)
        {
            List<string> barcodes = new List<string>();
            var toBodies = await _toBodyRepository.GetAllListAsync(p => p.TOHeaderId == input.OrderId);
            //拆分传入的购票信息
            var ticketInput = SeparateTicket(input.GroupInputs);

            long invoiceNo = long.Parse(input.InvoiceInput.InvoiceNo);

            //var zeroLength = input.InvoiceInput.InvoiceNo.Length - invoiceNo.ToString().Length;

            foreach (var item in ticketInput)
            {
                var toBody = toBodies.FirstOrDefault(p => p.AgencySaleTicketClassId == item.AgencySaleTicketClassId);
                if (toBody == null)
                    return Result.FromCode<List<string>>(ResultCode.Fail);

                TOVoucher entity = new TOVoucher()
                {
                    Id = await _uniqueCode.CreateAsync(CodeType.Voucher, toBody.ParkId),
                    TOBodyId = toBody.Id,
                    CustomerId = toBody.CustomerId,
                    ParkId = toBody.ParkId,
                    Seq = toBody.Seq,
                };

                //插入凭证
                await _toVoucherRepository.InsertAndGetIdAsync(entity);

                //插入发票
                var result = await _invoiceDomainService.AddInvoiceAndReturnIdAsync(new Invoice()
                {
                    Barcode = entity.Id,
                    InvoiceNo = invoiceNo.ToString(new string('0', input.InvoiceInput.InvoiceNo.Length)),
                    InvoiceCode = input.InvoiceInput.InvoiceCode,
                    IsActive = true,
                    TerminalId = terminalId
                });

                if (result.Success == false)
                    return Result.FromCode<List<string>>(result.Code);

                //插入取票记录
                var toticket = new TOTicket
                {
                    ParkId = toBody.ParkId,
                    Qty = item.Qty,
                    AgencySaleTicketClassId = toBody.AgencySaleTicketClassId,
                    ValidStartDate = toBody.TOHeader.ValidStartDate,
                    //旅行社取票默认当天
                    ValidDays = 0,
                    Price = toBody.Price,
                    SalePrice = toBody.SalePrice,
                    SettlementPrice = toBody.SettlementPrice,
                    Amount = item.Qty * toBody.SalePrice,
                    TicketSaleStatus = TicketSaleStatus.Valid,
                    InvoiceId = result.Data,
                    TOVoucherId = entity.Id,
                };

                await _toTicketDomainService.AddTOTicketAsync(toticket, terminalId);
                //增加打印条码
                barcodes.Add(toticket.Id);

                //更新子订单状态为已消费
                await _iTOBodyDomainService.ConsumedTOBody(toBody.Id);

                invoiceNo = invoiceNo + 1;

                //同步套票数据
                if (toBody.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.MultiParkTicket)
                {
                    await MultiTicketDataSync(entity.Id, terminalId);
                }
            }

            //订单核销
            foreach (var tobody in toBodies)
            {
                OrderConsume(new OrderConsumeDto() { SubOrderid = tobody.Id });
            }

            return Result.FromData(barcodes);

        }

        /// <summary>
        /// 根据查询条件获取网络订单信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetTOTicketAsync<TDto>(IQuery<TOTicket> query)
        {
            return await _toTicketRepository.AsNoTracking().FirstOrDefaultAsync<TOTicket, TDto>(query);
        }

        /// <summary>
        /// 根据条码获取网络订票信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;GroupTicket&gt;.</returns>
        public async Task<TOTicket> GetTOTicketByBarcodeAsync(string barcode)
        {
            if (!await _toTicketRepository.GetAll().AnyAsync(o => o.Id == barcode))
            {
                return null;
            }

            return await _toTicketRepository.AsNoTrackingAndInclude(o => o.TicketInParks, o => o.Invoice, o => o.AgencySaleTicketClass.Agency).FirstOrDefaultAsync(o => o.Id == barcode);
        }

        /// <summary>
        /// 根据条码确定门票是否未使用
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CheckTicketUnusedAsync(string barcode)
        {
            var date = DateTime.Today;

            return _toTicketRepository.GetAll().AnyAsync(o => o.Id == barcode && o.TicketSaleStatus == TicketSaleStatus.Valid
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
            var ticket = await _toTicketRepository.GetAsync(barcode);

            if (ticket.ParkId != AbpSession.LocalParkId)
            {
                return Result.FromCode(ResultCode.Fail, "套票不支持手动入园。");
            }

            var ticketClass = await _agencySaleTicketClassRepository.GetAllIncluding(o => o.ParkSaleTicketClass.TicketClass)
                .Where(o => o.Id == ticket.AgencySaleTicketClassId)
                .Select(o => o.ParkSaleTicketClass.TicketClass).SingleAsync();

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

            var data = new OrderConsumeDto() { SubOrderid = ticket.TOVoucher.TOBodyId };
            var tobodyDomainAppservice = IocManager.Instance.Resolve<ITOBodyDomainService>();
            await tobodyDomainAppservice.ConsumedTOBody(data.SubOrderid);

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
        /// 根据查询条件获取网络订单信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetTOTicketListAsync<TDto>(IQuery<TOTicket> query)
        {
            return await _toTicketRepository.AsNoTracking().ToListAsync<TOTicket, TDto>(query);
        }

        ///// <summary>
        ///// 退票后更改状态
        ///// </summary>
        ///// <param name="barCode"></param>
        ///// <returns></returns>
        //public async Task<Result<string>> UpdatTOTicketToInvalidAsync(string barCode)
        //{
        //    var entity = await _toTicketRepository.FirstOrDefaultAsync(barCode);

        //    if (entity != null)
        //    {
        //        entity.TicketSaleStatus = TicketSaleStatus.Refund;

        //        return Result.Ok();
        //    }

        //    return Result.FromCode(ResultCode.NoRecord);
        //}

        #endregion

        /// <summary>
        /// 取网络票业务验证
        /// </summary>
        /// <returns></returns>
        private async Task<Result<List<TOTicket>>> TakeWebTicketValidate(string tobodyId)
        {
            var tobody = await _toBodyRepository.GetAsync(tobodyId);

            //订单状态验证
            if (tobody.OrderState != OrderState.WaitCost)
                return Result.FromCode<List<TOTicket>>(ResultCode.Fail, tobody.OrderState.DisplayName());
            //作废电子票
            var etickets = await _toTicketRepository.GetAllIncluding(p => p.TOVoucher).Where(p => p.TOVoucher.TOBodyId == tobodyId && p.TicketSaleStatus == TicketSaleStatus.Valid).ToListAsync();
            if (etickets == null || etickets.Count == 0)
                return Result.FromCode<List<TOTicket>>(ResultCode.NoRecord);

            //验证票类是否存在
            if (etickets.Any(p => p.AgencySaleTicketClass == null))
                return Result.FromCode<List<TOTicket>>(ResultCode.MissTicketType);
            return Result.FromData(etickets);
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
                        TicketClassMode = saleTicket.TicketClassMode
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
                        TicketClassMode = saleTicket.TicketClassMode
                    };

                    printTicket.Add(ticket);
                }
            }
            return printTicket;
        }

        /// <summary>
        /// 套票同步
        /// </summary>
        private async Task MultiTicketDataSync(string voucherId, int terminalId)
        {
            var ticketdetails = await _toTicketRepository.GetAll().Include(o => o.AgencySaleTicketClass).Where(p => p.TOVoucherId == voucherId && p.TicketSaleStatus == TicketSaleStatus.Valid).ToListAsync();
            var localParkId = await SettingManager.GetSettingValueAsync(DataSyncSetting.LocalParkId);

            foreach (var ticketdetail in ticketdetails)
            {
                var dto = new MultiTicketSendDto
                {
                    TicketClassType = TicketCategory.Order,
                    Barcode = ticketdetail.Id,
                    Amount = ticketdetail.Amount,
                    FromParkid = ticketdetail.ParkId,
                    Price = ticketdetail.Price,
                    Qty = ticketdetail.Qty,
                    SalePrice = ticketdetail.SalePrice,
                    TerminalId = terminalId,
                    ValidDays = ticketdetail.ValidDays,
                    ValidStartDate = ticketdetail.ValidStartDate,
                    AgencySaleTicketClassId = ticketdetail.AgencySaleTicketClassId,
                    TOVoucherId = ticketdetail.TOVoucherId,
                    CreatorUserId = AbpSession.UserId,
                    ParkSettlementPrice = ticketdetail.ParkSettlementPrice,
                    SettlementPrice = ticketdetail.SettlementPrice,

                    CreationTime = ticketdetail.CreationTime,
                    InvoiceId = ticketdetail.InvoiceId,
                    LastModificationTime = ticketdetail.LastModificationTime,
                    LastModifierUserId = ticketdetail.LastModifierUserId,
                    TicketFormEnum = ticketdetail.TicketFormEnum

                };

                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MultiTicketSend,
                    SyncData = JsonConvert.SerializeObject(dto)
                };
                var parkSaleTicketClass = await _parkSaleTicketClassRepository.FirstOrDefaultAsync(p => p.Id == ticketdetail.AgencySaleTicketClass.ParkSaleTicketClassId);
                var otherParkIds = parkSaleTicketClass.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != localParkId)
                    {
                        _dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
            }

        }


        private async Task CreateVipCardVoucher(List<TOTicket> vipVouchers,int terminalId)
        {
            foreach (var toticket in vipVouchers)
            {
                var validDays = _nonGroupTicketAppService.GetValidDays(toticket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClassId);

                //网络票年卡激活先生成凭证
                var entityVipVoucher = new VIPVoucher
                {
                    State = VipVoucherStateType.NotActive,
                    ParkSaleTicketClassId = toticket.AgencySaleTicketClass.ParkSaleTicketClassId,
                    SalePrice = toticket.SalePrice,
                    ValidDateBegin = DateTime.Now.Date,
                    ValidDateEnd = DateTime.Now.Date.AddDays(validDays.Result),
                    TerminalId = terminalId,
                    Barcode = toticket.Id,
                    ParkId = AbpSession.LocalParkId
                };
                await _parkVipVoucherRepository.InsertAndGetIdAsync(entityVipVoucher);
            }

        }

        /// <summary>
        /// 订单核销
        /// </summary>
        /// <returns></returns>
        private void OrderConsume(OrderConsumeDto data)
        {
            DataSyncInput input = new DataSyncInput()
            {
                SyncData = JsonConvert.SerializeObject(data),
                SyncType = DataSyncType.OrderConsume
            };
            _dataSyncManager.UploadDataToTargetPark(ThemeParkConsts.CenterParkId, input);
        }


        /// <summary>
        /// 获取代理商的有效日期
        /// </summary>
        /// <returns></returns>
        private int GetValidDays(int agencyTypeId, int agencyId)
        {
            var defaultResult = _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == agencyTypeId).Result ?? new DefaultPrintSet();
            var validDays = defaultResult.DefaultValidDays;

            //获取代理商单独配置项
            var groupResult = _agencyPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyId == agencyId).Result;
            if (groupResult?.ValidDays == null)
                return validDays;
            //如果有团体配置，覆盖默认的打印价格类型和有效期配置
            validDays = groupResult.ValidDays.Value;
            return validDays;
        }
    }
    /// <summary>
    /// 比较长期票是否与活动票重复
    /// </summary>
    public class IsTOHeaderEqual : IEqualityComparer<GetWebTicketOrderDto>
    {
        public bool Equals(GetWebTicketOrderDto x, GetWebTicketOrderDto y)
        {
            if (x == null)
                return y == null;
            return x.OrderId == y.OrderId;
        }

        public int GetHashCode(GetWebTicketOrderDto obj)
        {
            if (obj == null)
                return 0;
            return obj.OrderId.GetHashCode();
        }
    }
}
