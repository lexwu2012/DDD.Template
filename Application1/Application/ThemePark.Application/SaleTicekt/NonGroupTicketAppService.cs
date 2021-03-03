using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.ParkSale.DomainServiceInterfaces;
using ThemePark.Infrastructure.Core;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Application.DataSync.Dto;
using Newtonsoft.Json;
using ThemePark.Application.DataSync.Interfaces;
using Abp.Dependency;
using Abp.EntityFramework.Uow;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Common;
using ThemePark.Core.BasicData;
using ThemePark.Core.DataSync;
using ThemePark.Core.InPark;
using ThemePark.Core.Settings;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 客户端散客售票服务
    /// </summary>
    public class NonGroupTicketAppService : ThemeParkAppServiceBase, INonGroupTicketAppService
    {
        #region Fields
        private readonly IParkSaleTicketClassAppService _parkSaleTicketClassAppService;
        private readonly IRepository<NonGroupTicket, string> _nonGroupTicketRepository;
        private readonly IInvoiceDomainService _invoiceDomainService;
        private readonly IUniqueCode _uniqueCode;
        private readonly ITradeInfoAppService _tradeInfoAppService;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<TicketInPark, long> _ticketinparkRepository;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;
        private readonly IRepository<DefaultPrintSet> _defaultPrintSetRepository;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;
        private readonly IRepository<TicketPrintSet> _ticketPrintSetRepository;
        #endregion

        #region cotr
        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="invoiceAppService"></param>
        /// <param name="parkSaleTicketClassAppServiceAppService"></param>
        /// <param name="nonGrouTicketRepository"></param>
        /// <param name="invoiceDomainService"></param>
        /// <param name="uniqueCode"></param>
        /// <param name="tradeInfoAppService"></param>
        /// <param name="ticketinparkRepository"></param>
        /// <param name="parkSaleTicketClassRepository"></param>
        /// <param name="settingManager"></param>
        /// <param name="defaultPrintSetRepository"></param>
        /// <param name="dataSyncManager"></param>
        /// <param name="invoiceCodeRepository"></param>
        public NonGroupTicketAppService(IParkSaleTicketClassAppService parkSaleTicketClassAppServiceAppService, ITradeInfoAppService tradeInfoAppService,
             IRepository<NonGroupTicket, string> nonGrouTicketRepository, IInvoiceDomainService invoiceDomainService, IUniqueCode uniqueCode,
             ISettingManager settingManager, IRepository<TicketInPark, long> ticketinparkRepository, IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository,
             IRepository<DefaultPrintSet> defaultPrintSetRepository, IDataSyncManager dataSyncManager, IInvoiceAppService invoiceAppService,
             IRepository<InvoiceCode> invoiceCodeRepository, IRepository<TicketPrintSet> ticketPrintSetRepository)
        {
            _parkSaleTicketClassAppService = parkSaleTicketClassAppServiceAppService;
            _nonGroupTicketRepository = nonGrouTicketRepository;
            _invoiceDomainService = invoiceDomainService;
            _uniqueCode = uniqueCode;
            _settingManager = settingManager;
            _ticketinparkRepository = ticketinparkRepository;
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _defaultPrintSetRepository = defaultPrintSetRepository;
            _tradeInfoAppService = tradeInfoAppService;
            _dataSyncManager = dataSyncManager;
            _invoiceAppService = invoiceAppService;
            _invoiceCodeRepository = invoiceCodeRepository;
            _ticketPrintSetRepository = ticketPrintSetRepository;
        }
        #endregion


        /// <summary>
        /// 新增购票记录
        /// </summary>
        /// <param name="saveNonGroupSaleTicketDto">需要添加的购票列表记录</param>
        /// <param name="terminalId">终端号</param>
        /// <param name="parkId">公园Id</param>
        /// <returns></returns>
        public async Task<Result<string>> AddNonGroupTicketAndReturnTradeNumAsync(SaveNonGroupSaleTicketDto saveNonGroupSaleTicketDto, int terminalId, int parkId)
        {
            //1. 业务验证
            var invoiceCode = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceCode);

            if (null == invoiceCode)
                return Result.FromError<string>("发票代码不存在");

            //重新组装列表数据
            var ticketInput = ReCombineTicket(saveNonGroupSaleTicketDto.TicketInfos);

            //业务规则验证
            var result = await BusinessRuleVerify(saveNonGroupSaleTicketDto, ticketInput.Count, invoiceCode.InvoiceNumIsIncrease);
            if (!result.Success)
                return result;

            //2. 支付
            var tradenoResult = await _tradeInfoAppService.AddTradeInfoAndReturnTradeInfoIdAsyn(saveNonGroupSaleTicketDto.TradeInfos, parkId, null);
            if (!tradenoResult.Success)
                return tradenoResult;

            //传入的invoiceInput.InvoceNo是有效可用的，即数据库最大数已经加上1了
            var invoiceNo = long.Parse(saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceNo);

            List<NonGroupTicket> nonGroupTickets = new List<NonGroupTicket>();
            List<NonGroupTicket> mulTickets = new List<NonGroupTicket>();
            List<OtherNonGroupTicket> otherTickets = new List<OtherNonGroupTicket>();

            //3. 生成票记录
            foreach (var input in ticketInput)
            {
                var entity = await CreateNonGroupTicket(input, parkId, terminalId, tradenoResult.Data, invoiceNo,
                    saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceCode, saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceNo.Length);

                if (!entity.Success)
                    return Result.FromCode<string>(entity.Code);

                //本公园票
                if (input.ParkId == parkId)
                    nonGroupTickets.Add(entity.Data);

                //需要同步到其他公园的套票数据
                if (input.TicketClassMode == TicketClassMode.MultiParkTicket)
                    mulTickets.Add(entity.Data);

                //他园票需要同步到其他公园
                if (input.ParkId != parkId)
                {
                    //他园票公园ID为目标公园ID
                    entity.Data.ParkId = input.ParkId;

                    otherTickets.Add(entity.Data.MapTo<OtherNonGroupTicket>());
                }

                //发票号递增
                if (invoiceCode.InvoiceNumIsIncrease)
                    invoiceNo = invoiceNo + 1;
                else
                    //发票号递减
                    invoiceNo = invoiceNo - 1;
            }

            //批量插入数据库
            await UnitOfWorkManager.Current.GetDbContext<ThemeParkDbContext>().BulkInsertAsync(nonGroupTickets);
            await UnitOfWorkManager.Current.GetDbContext<ThemeParkDbContext>().BulkInsertAsync(otherTickets);
            await CurrentUnitOfWork.SaveChangesAsync();
            IocManager.Instance.Resolve<IFixedRedisCache>().InvalidateSets(new[] { nameof(NonGroupTicket), nameof(OtherNonGroupTicket) });
            //4. 套票同步
            foreach (var entity in mulTickets)
            {
                await MultiTicketDataSync(entity, parkId, terminalId);
            }

            //5数据同步他园票
            SyncOtherNonGroupTicket(otherTickets, parkId);

            return tradenoResult;
        }

        public async Task<NonGroupTicket> GetTicketById(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await _nonGroupTicketRepository.FirstOrDefaultAsync(o => o.Id == id);
        }


        /// <summary>
        /// 根据查询条件获取散客票信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetNonGroupTicketAsync<TDto>(IQuery<NonGroupTicket> query)
        {
            return await _nonGroupTicketRepository.AsNoTracking().FirstOrDefaultAsync<NonGroupTicket, TDto>(query);
        }

        /// <summary>
        /// 根据条码获取散客票信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;NonGroupTicket&gt;.</returns>
        public async Task<NonGroupTicket> GetNonGroupTicketByBarcodeAsync(string barcode)
        {
            if (!await _nonGroupTicketRepository.GetAll().AnyAsync(o => o.Id == barcode))
            {
                return null;
            }

            return await _nonGroupTicketRepository.AsNoTrackingAndInclude(o => o.TicketInParks, o => o.Invoice).FirstOrDefaultAsync(o => o.Id == barcode);
        }


        /// <summary>
        /// 根据条码确定门票是否未使用
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CheckTicketUnusedAsync(string barcode)
        {
            var date = DateTime.Now.Date;

            return _nonGroupTicketRepository.GetAll().AnyAsync(o => o.Id == barcode && o.TicketSaleStatus == TicketSaleStatus.Valid
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
            var ticket = await _nonGroupTicketRepository.GetAsync(barcode);

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
            _ticketinparkRepository.Insert(new TicketInPark()
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
        /// 根据查询条件获取散客票信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetNonGroupTicketListAsync<TDto>(IQuery<NonGroupTicket> query)
        {
            return await _nonGroupTicketRepository.AsNoTracking().ToListAsync<NonGroupTicket, TDto>(query);
        }

        /// <summary>
        /// 验证散客票支付数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<decimal>> CheckNonGroupTicketAsync(ISaleTicketCheck<NonGroupTicketInput> input)
        {
            decimal amount = 0;
            foreach (var ticketSaleInfo in input.TicketInfos)
            {
                var ticketData = await _parkSaleTicketClassAppService.GetOnCacheByIdAsync(ticketSaleInfo.TicketClassId);
                amount += ticketData.SalePrice * ticketSaleInfo.Qty;
            }
            //验证各个支付方式支付金额之和是否等于支付总金额
            decimal? payDetailSum = input.TradeInfos.TradeInfoDetails.Sum(tradeInfodetail => tradeInfodetail?.Amount);
            if (payDetailSum != amount)
                return Result.FromCode<decimal>(ResultCode.InvalidData);

            //附上正确的后台计算金额
            input.TradeInfos.Amount = amount;

            return Result.FromData(amount);
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateNonGroupTicketToInvalidAndReturnOriginalTradeIdAsync(string barCode)
        {
            var entity = await _nonGroupTicketRepository.FirstOrDefaultAsync(barCode);

            if (entity != null)
            {
                entity.TicketSaleStatus = TicketSaleStatus.Refund;

                return Result.FromData(entity.TradeInfoId);
            }

            return Result.FromError<string>(ResultCode.NoRecord.DisplayName());
        }

        /// <summary>
        /// 获取散客票默认有效期
        /// </summary>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        public async Task<Result<int>> GetNonGroupValidAsync()
        {
            var value = await _settingManager.GetSettingValueAsync(TicketSetting.NonGroupValid);
            return Result.FromData(int.Parse(value));
        }

        #region Private Methods

        /// <summary>
        /// 创建散客票实体
        /// </summary>
        /// <returns></returns>
        private async Task<Result<NonGroupTicket>> CreateNonGroupTicket(NonGroupTicketInput input, int parkId, int terminalId, string tradeNo,
            long invoiceNo, string invoiceCode, int invoiceNoLength)
        {
            //价格数据从数据库获取赋值
            var parkSaleTicket = await _parkSaleTicketClassAppService.GetOnCacheByIdAsync(input.ParkSaleTicketClassId);
            //打印模板绑定的票类ID
            var printSetTicketClassId = parkSaleTicket.TicketClassId;
            //他园票打印模板取本公园相同TicketTypeId的票类
            if (input.ParkId != parkId)
            {
                var parkSaleTicketClass = await _parkSaleTicketClassRepository.FirstOrDefaultAsync(p =>
                      p.TicketClass.TicketTypeId == parkSaleTicket.TicketTypeId &&
                      p.ParkId == parkId);
                printSetTicketClassId = parkSaleTicketClass.TicketClassId;
            }
            var entity = input.MapTo<NonGroupTicket>();
            //barCode
            entity.Id = await _uniqueCode.CreateAsync(CodeType.Barcode, parkId, terminalId);

            entity.ParkId = parkId;
            entity.TerminalId = terminalId;
            entity.TradeInfoId = tradeNo;
            entity.Price = parkSaleTicket.Price;
            entity.SalePrice = parkSaleTicket.SalePrice;
            entity.Amount = parkSaleTicket.SalePrice * input.Qty;
            //获取配置有效期
            entity.ValidDays = GetValidDays(printSetTicketClassId).Result;
            entity.ValidStartDate = input.ValidStartDate.Date;
            entity.CreatorUserId = AbpSession.GetUserId();
            //写入发票记录
            var invoiceResult = await _invoiceDomainService.AddInvoiceAndReturnIdAsync(new Invoice()
            {
                Barcode = entity.Id,
                InvoiceNo = invoiceNo.ToString(new string('0', invoiceNoLength)),
                InvoiceCode = invoiceCode,
                IsActive = true,
                TerminalId = terminalId
            });
            if (!invoiceResult.Success)
                return Result.FromCode<NonGroupTicket>(invoiceResult.Code);
            entity.InvoiceId = invoiceResult.Data;
            entity.TicketSaleStatus = TicketSaleStatus.Valid;

            return Result.FromData(entity);
        }


        /// <summary>
        /// 拆分购票数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<NonGroupTicketInput> ReCombineTicket(List<NonGroupTicketInput> input)
        {
            List<NonGroupTicketInput> printTicket = new List<NonGroupTicketInput>();

            foreach (var saleTicket in input)
            {
                //窗口列表每条记录的票总数
                int qty = saleTicket.Qty;

                //多少张票合并一张
                int numPerTicket = saleTicket.IsAllOutPut;

                for (int i = 0; i < qty / numPerTicket; i++)
                {
                    var ticket = new NonGroupTicketInput()
                    {
                        Qty = numPerTicket,
                        ParkSaleTicketClassId = saleTicket.ParkSaleTicketClassId,
                        ValidStartDate = saleTicket.ValidStartDate,
                        TicketClassMode = saleTicket.TicketClassMode,
                        ParkId = saleTicket.ParkId
                    };

                    printTicket.Add(ticket);
                }

                //没整除（分票输出）的时候所剩余数作为一张票
                if (qty % numPerTicket != 0)
                {
                    var ticket = new NonGroupTicketInput()
                    {
                        Qty = qty % numPerTicket,
                        ParkSaleTicketClassId = saleTicket.ParkSaleTicketClassId,
                        ValidStartDate = saleTicket.ValidStartDate,
                        TicketClassMode = saleTicket.TicketClassMode,
                        ParkId = saleTicket.ParkId
                    };

                    printTicket.Add(ticket);
                }
            }
            return printTicket;
        }

        /// <summary>
        /// 散客售票业务规则验证
        /// </summary>
        /// <param name="saveNonGroupSaleTicketDto"></param>
        /// <param name="ticketCount"></param>
        /// <param name="invoiceNumIsIncrease"></param>
        /// <returns></returns>
        private async Task<Result<string>> BusinessRuleVerify(SaveNonGroupSaleTicketDto saveNonGroupSaleTicketDto, int ticketCount, bool invoiceNumIsIncrease)
        {
            //判断手动输入的入园日期是否合理
            if (saveNonGroupSaleTicketDto.TicketInfos.Any(item => item.ValidStartDate.Date < DateTime.Now.Date))
                return Result.FromError<string>("入园日期不能早于今天，请重新选择！");

            //输出票数不能大过总数量
            if (saveNonGroupSaleTicketDto.TicketInfos.Any(m => m.IsAllOutPut > m.Qty))
                return Result.FromCode<string>(ResultCode.InvalidData);

            // 一张票对应一张发票
            if (string.IsNullOrWhiteSpace(saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceNo) || string.IsNullOrWhiteSpace(saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceCode))
                return Result.FromCode<string>(ResultCode.MissEssentialData, "发票号或者发票代码不能为空");

            //总额验证
            var result = await CheckNonGroupTicketAsync(saveNonGroupSaleTicketDto);
            if (!result.Success)
                return Result.FromCode<string>(ResultCode.InvalidData, "金额数目不正确");

            //检测是否存在重复发票号或者负数
            var existed = _invoiceAppService.CheckIfExisteInValidOrDuplicateInvoice(saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceCode,
                saveNonGroupSaleTicketDto.InvoiceInfos.InvoiceNo, ticketCount, invoiceNumIsIncrease);

            if (existed)
                return Result.FromCode<string>(ResultCode.DuplicateInvoiceRecord);

            return Result.FromCode<string>(ResultCode.Ok);
        }

        /// <summary>
        /// 套票同步
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        private async Task MultiTicketDataSync(NonGroupTicket entity, int parkId, int terminalId)
        {
            //同步套票数据
            //var ticketdetail = await _nonGroupTicketRepository.GetAll().Include(o => o.Park).Include(o => o.ParkSaleTicketClass).FirstOrDefaultAsync(p => p.Id == entity.Id);
            var dto = new MultiTicketSendDto
            {
                TicketClassType = TicketCategory.NonGroupTicket,
                Barcode = entity.Id,
                Amount = entity.Amount,
                FromParkid = parkId,
                ParkSaleTicketClassId = entity.ParkSaleTicketClassId,
                Price = entity.Price,
                Qty = entity.Qty,
                SalePrice = entity.SalePrice,
                TerminalId = terminalId,
                ValidDays = entity.ValidDays,
                ValidStartDate = entity.ValidStartDate,
                TradeInfoId = entity.TradeInfoId,
                CreatorUserId = AbpSession.UserId,

                InvoiceId = entity.InvoiceId,
                CustomerId = entity.CustomerId,
                CreationTime = entity.CreationTime,
                LastModificationTime = entity.LastModificationTime,
                LastModifierUserId = entity.LastModifierUserId,
                SyncTicketType = entity.SyncTicketType
            };
            var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
            var syncInput = new DataSyncInput
            {
                SyncType = DataSyncType.MultiTicketSend,
                SyncData = JsonConvert.SerializeObject(dto)
            };

            //var result = await _parkSaleTicketClassAppService.GetMulSaleTicketByParkIdsAsync<ParkSaleTicketClassCacheItem>(new List<int>() { parkId }, null);
            //var otherParkIds = result?.Where(p => p.Id == entity.ParkSaleTicketClassId).First().InParkIdFilter.Trim().Split(',');
            var parkSaleTicketClass = await _parkSaleTicketClassRepository.FirstOrDefaultAsync(p => p.Id == entity.ParkSaleTicketClassId);
            var otherParkIds = parkSaleTicketClass?.TicketClass.InParkIdFilter.Split(',');
            foreach (var parkid in otherParkIds)
            {
                if (parkid != parkId.ToString())
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                }
            }
        }


        /// <summary>
        /// 他园票数据同步
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="fromParkId"></param>
        private void SyncOtherNonGroupTicket(List<OtherNonGroupTicket> entities, int fromParkId)
        {
            foreach (var entity in entities)
            {
                var toParkId = entity.ParkId;
                //数据同步公园ID为卖方ID
                entity.ParkId = fromParkId;
                var syncData = entity.MapTo<OtherTicketSendDto>();
                var syncInput = new DataSyncInput()
                {
                    SyncData = JsonConvert.SerializeObject(syncData),
                    SyncType = DataSyncType.OtherTicketSend
                };
                _dataSyncManager.UploadDataToTargetPark(toParkId, syncInput);
            }
        }


        /// <summary>
        /// 获取散客有效期
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetValidDays(int ticketClassId)
        {
            var defaultResult = _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == null).Result ?? new DefaultPrintSet();
            var validDays = defaultResult.DefaultValidDays;
            var ticketPrintSet = await _ticketPrintSetRepository.FirstOrDefaultAsync(p => p.TicketClassId == ticketClassId);
            if (ticketPrintSet?.ValidDays != null)
                validDays = ticketPrintSet.ValidDays.Value;
            return validDays;
        }

        #endregion
    }
}
