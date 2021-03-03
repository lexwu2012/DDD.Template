using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Uow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Application.DataSync;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.OrderTrack.Dto;
using ThemePark.Application.OrderTrack.Interface;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.DataSync;
using ThemePark.Core.ParkSale;
using ThemePark.Core.ParkSale.DomainServiceInterfaces;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Core;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Application.SaleTicekt
{
    public class VendorTicketAppService : ThemeParkAppServiceBase, IVendorTicketAppService
    {
        private readonly ITradeInfoAppService _tradeInfoAppService;
        private readonly IVendorSetAppService _vendorSetAppService;
        private readonly IRepository<OtherNonGroupTicket, string> _otherNonGroupTicketRepository;
        private readonly IUniqueCode _uniqueCode;
        private readonly IParkSaleTicketClassAppService _parkSaleTicketClassAppService;
        private readonly IInvoiceDomainService _invoiceDomainService;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IRepository<TOBody, string> _toBodyRepository;
        private readonly IRepository<SyncPark> _syncParkRepository;
        private readonly ITOTicketAppService _toTicketAppService;
        private readonly IRepository<NonGroupTicket, string> _nonGroupTicketRepository;
        private readonly INonGroupTicketAppService _nonGroupTicketAppService;
        private readonly IRepository<DefaultPrintSet> _defaultPrintSetRepository;
        private readonly IVendorTicketTrackAppService _vendorTicketTrackAppService;
        private readonly IPrintTicketAppService _printTicketAppService;
        private readonly IRepository<TicketPrintSet> _ticketPrintSetRepository;


        public VendorTicketAppService(ITradeInfoAppService tradeInfoAppService, IVendorSetAppService vendorSetAppService, IUniqueCode uniqueCode, IParkSaleTicketClassAppService parkSaleTicketClassAppService, IInvoiceDomainService invoiceDomainService, IDataSyncManager dataSyncManager, IRepository<OtherNonGroupTicket, string> otherNonGroupTicketRepository, IRepository<TOBody, string> toBodyRepository, IRepository<SyncPark> syncParkRepository, ITOTicketAppService toTicketAppService, IPrintTicketAppService printTicketAppService, INonGroupTicketAppService nonGroupTicketAppService, IRepository<NonGroupTicket, string> nonGroupTicketRepository, IRepository<DefaultPrintSet> defaultPrintSetRepository, IVendorTicketTrackAppService vendorTicketTrackAppService, IPrintTicketAppService printTicketAppService1, IRepository<TicketPrintSet> ticketPrintSetRepository)
        {
            _tradeInfoAppService = tradeInfoAppService;
            _vendorSetAppService = vendorSetAppService;
            _uniqueCode = uniqueCode;
            _parkSaleTicketClassAppService = parkSaleTicketClassAppService;
            _invoiceDomainService = invoiceDomainService;
            _dataSyncManager = dataSyncManager;
            _otherNonGroupTicketRepository = otherNonGroupTicketRepository;
            _toBodyRepository = toBodyRepository;
            _syncParkRepository = syncParkRepository;
            _toTicketAppService = toTicketAppService;
            _nonGroupTicketAppService = nonGroupTicketAppService;
            _nonGroupTicketRepository = nonGroupTicketRepository;
            _defaultPrintSetRepository = defaultPrintSetRepository;
            _vendorTicketTrackAppService = vendorTicketTrackAppService;
            _printTicketAppService = printTicketAppService1;
            _ticketPrintSetRepository = ticketPrintSetRepository;
        }


        /// <summary>
        /// 获取本公园自助售票机预订票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<List<SearchVendorOrderDto>>> SearchVendorTicket(SearchVendorTicketOrderInput input)
        {
            var plandate = DateTime.Now.Date;
            var search = new Query<TOBody>(
                //pidOrTicketCode等于身份证或订单ID
                p => (p.Customer.Pid == input.PidOrTicketCode || p.TOHeaderId == input.PidOrTicketCode) && (p.TOHeader.ValidStartDate >= plandate || p.TOHeader.ValidEndDate >= plandate));
            //订单状态为待消费
            //(p.OrderState == OrderState.WaitCost) &&
            //p.TOHeader.ValidStartDate >= plandate &&
            //p.TOHeader.OrderType == OrderType.OTAOrder);

            var result = await _toBodyRepository.GetAllListAsync(search.GetFilter());
            //订单不存在
            if (result.Count == 0)
                return Result.FromCode<List<SearchVendorOrderDto>>(ResultCode.VendorOrderNoExists);
            result = result.Where(p => p.OrderState == OrderState.WaitCost).ToList();

            //订单已消费或冻结
            if (result.Count == 0)
                return Result.FromCode<List<SearchVendorOrderDto>>(ResultCode.VendorOrderConsumed);
            return Result.FromData(result.MapTo<List<SearchVendorOrderDto>>());
        }


        /// <summary>
        /// 自助售票机获取其他公园预订票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<List<SearchVendorOrderDto>>> SearchOtherParkVendorTicket(SearchOtherParkVendorOrderInput input)
        {
            List<SearchVendorOrderDto> result = new List<SearchVendorOrderDto>();
            foreach (var parkId in input.ParkIds)
            {
                var sync = await _syncParkRepository.GetAll().FirstOrDefaultAsync(o => o.ParkId == parkId);
                if (sync == null)
                    continue;
                var uri = new Uri(sync.SyncUrl);
                Result<List<SearchVendorOrderDto>> data;
                try
                {
                    var response =
                        await
                            HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/VendorTicket/SearchVendorTicket",
                            new JObject(new JProperty("PidOrTicketCode", input.PidOrTicketCode)).ToString());
                    data = JsonConvert.DeserializeObject<Result<List<SearchVendorOrderDto>>>(response);
                }
                catch (Exception ex)
                {
                    return Result.FromCode<List<SearchVendorOrderDto>>(ResultCode.Fail, "订单未找到，网络连接异常");
                }
                //公园下架后数据为空
                if (data != null && data.Success && data.Data.Count > 0)
                    result.AddRange(data.Data);
            }
            if (result.Count <= 0)
                return Result.FromCode<List<SearchVendorOrderDto>>(ResultCode.NoRecord);
            return Result.FromData(result);

        }


        /// <summary>
        /// 自助售票机取本公园网络票
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qty"></param>
        /// <param name="localParkId"></param>
        /// <returns></returns>
        public async Task<Result<List<PrintInfo>>> TakeVendorTicket(ToTicketInput input, int qty, int localParkId)
        {
            Result<List<PrintInfo>> result;
            if (input.ParkId == localParkId)
                result = await TakeLocalParkVendorTicket(input);
            else
                result = await TakeOtherParkVendorTicket(input);
            await _vendorSetAppService.UpdateTicketMax(input.TerminalId, qty);
            return result;
        }




        /// <summary>
        /// 自助售票机本公园售票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddVendorTicketAsync(AddVendorTicketInput input)
        {
            var qty = 0;
            input.TicketInfos.ForEach(p => qty += p.Qty);
            var check = await IsTicketEnough(qty, input.TerminalId);
            //验证票数是否足够
            if (!check.Success)
                return Result.FromCode<string>(check.Code, check.Message);

            //支付
            var tradenoResult = await _tradeInfoAppService.AddTradeInfoAndReturnTradeInfoIdAsyn(input.TradeInfos, input.ParkId, null);
            if (!tradenoResult.Success)
                return tradenoResult;

            //追踪支付
            await _vendorTicketTrackAppService.PayVendorTicketTrack(input.TradeInfos, new TicketTrackInput() { ParkId = input.ParkId, TerminalId = input.TerminalId, TradeinfoId = tradenoResult.Data });

            List<NonGroupTicket> nonGroupTickets = new List<NonGroupTicket>();
            List<NonGroupTicket> mulTickets = new List<NonGroupTicket>();
            //拆分，分票输出
            var sepInputs = SingleNongGroupTicketInput(input.TicketInfos);
            foreach (var item in sepInputs)
            {
                //构造实体
                var entity = await InitialNonGroupTicket(item, input.ParkId, input.TerminalId, tradenoResult.Data);
                nonGroupTickets.Add(entity);
                if (item.TicketClassMode == TicketClassMode.MultiParkTicket)
                    mulTickets.Add(entity);
            }

            //批量插入数据库
            await UnitOfWorkManager.Current.GetDbContext<ThemeParkDbContext>().BulkInsertAsync(nonGroupTickets);
            await CurrentUnitOfWork.SaveChangesAsync();
            IocManager.Instance.Resolve<IFixedRedisCache>().InvalidateSets(new[] { nameof(NonGroupTicket) });
            //套票同步
            foreach (var entity in mulTickets)
            {
                await MultiTicketDataSync(entity, input.ParkId, input.TerminalId);
            }

            //更新售票机发票配置
            await _vendorSetAppService.UpdateTicketMax(input.TerminalId, qty);

            return tradenoResult;

        }

        /// <summary>
        /// 新增他园票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddOtherParkVendorTicketAsync(AddOtherParkVendorTicketInput input)
        {
            var qty = 0;
            input.TicketInfos.ForEach(p => qty += p.Qty);
            var check = await IsTicketEnough(qty, input.TerminalId);
            //验证票数是否足够
            if (!check.Success)
                return Result.FromCode<string>(check.Code, check.Message);

            //支付
            var tradenoResult = await _tradeInfoAppService.AddTradeInfoAndReturnTradeInfoIdAsyn(input.TradeInfos, input.ParkId, null);
            if (!tradenoResult.Success)
                return Result.FromCode<string>(tradenoResult.Code);
            //支付追踪
            await _vendorTicketTrackAppService.PayVendorTicketTrack(input.TradeInfos, new TicketTrackInput() { ParkId = input.ParkId, TerminalId = input.TerminalId, TradeinfoId = tradenoResult.Data });

            //分票输出
            var inputs = SingleOtherNongGroupTicketInput(input.TicketInfos);
            List<OtherNonGroupTicket> entities = new List<OtherNonGroupTicket>();
            foreach (var item in inputs)
            {
                var entity = await InitialOtherNonGroupTicket(item, input.ParkId, input.TerminalId, tradenoResult.Data);
                entities.Add(entity);
            }
            //批量插入
            await UnitOfWorkManager.Current.GetDbContext<ThemeParkDbContext>().BulkInsertAsync(entities);
            
            //更新售票机发票配置
            await _vendorSetAppService.UpdateTicketMax(input.TerminalId, qty);

            //数据同步他园票
            SyncOtherNonGroupTicket(entities, input.FromParkId);
            await CurrentUnitOfWork.SaveChangesAsync();
            IocManager.Instance.Resolve<IFixedRedisCache>().InvalidateSets(new[] { nameof(NonGroupTicket) });

            return tradenoResult;

        }


        /// <summary>
        /// 自助售票机取其他公园网络票
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        private async Task<Result<List<PrintInfo>>> TakeOtherParkVendorTicket(ToTicketInput input)
        {
            var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == input.ParkId);
            Result<List<PrintInfo>> printInfos;
            var uri = new Uri(sync.SyncUrl);
            try
            {
                //创建用户的ID
                input.CreatorUserId = AbpSession.UserId;
                var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/VendorTicket/TakeVendorTicket", JsonConvert.SerializeObject(input));
                printInfos = JsonConvert.DeserializeObject<Result<List<PrintInfo>>>(response);
            }
            catch (Exception ex)
            {
                return Result.FromCode<List<PrintInfo>>(ResultCode.Fail, "取票失败，网络连接异常");
            }

            return printInfos;
        }

        /// <summary>
        /// 自助售票机取本公园票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Result<List<PrintInfo>>> TakeLocalParkVendorTicket(ToTicketInput input)
        {
            var result = await _toTicketAppService.AddWebTicketAsync(input, AbpSession.TerminalId);
            if (!result.Success)
                return Result.FromCode<List<PrintInfo>>(result.Code);
            var printInfos = new List<PrintInfo>();
            result.Data.ForEach(p => printInfos.AddRange(_printTicketAppService.GetPrintContentTask(PrintTicketType.WebTicket, barcode: p).Result.Data));
            return Result.FromData(printInfos);
        }

        /// <summary>
        /// 剩余票数是否足够
        /// </summary>
        /// <returns></returns>
        private async Task<Result> IsTicketEnough(int qty, int terminalId)
        {
            var dto = await _vendorSetAppService.GetVendorSet(terminalId);
            if (dto.Data.TicketMax < qty)
                return Result.FromCode(ResultCode.VendorTicketNotEnough);
            return Result.Ok();
        }


        /// <summary>
        /// 构造实体,分票输出
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private List<OtherNonGroupTicketInput> SingleOtherNongGroupTicketInput(List<OtherNonGroupTicketInput> inputs)
        {
            var dtos = new List<OtherNonGroupTicketInput>();
            foreach (var input in inputs)
            {
                for (int i = 0; i < input.Qty; i++)
                {
                    var dto = new OtherNonGroupTicketInput()
                    {
                        Qty = 1,
                        ParkSaleTicketClassId = input.ParkSaleTicketClassId,
                        ValidStartDate = input.ValidStartDate
                    };
                    dtos.Add(dto);
                }
            }
            return dtos;
        }

        /// <summary>
        /// 构造实体,分票输出
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private List<NonGroupTicketInput> SingleNongGroupTicketInput(List<NonGroupTicketInput> inputs)
        {

            var result = new List<NonGroupTicketInput>();
            foreach (var input in inputs)
            {
                for (int i = 0; i < input.Qty; i++)
                {
                    var dto = new NonGroupTicketInput()
                    {
                        ParkSaleTicketClassId = input.ParkSaleTicketClassId,
                        IsAllOutPut = input.IsAllOutPut,
                        Qty = 1,
                        TicketClassMode = input.TicketClassMode,
                        ValidStartDate = input.ValidStartDate
                    };
                    result.Add(dto);
                }
            }
            return result;
        }

        /// <summary>
        /// 创建散客票实体
        /// </summary>
        /// <returns></returns>
        private async Task<NonGroupTicket> InitialNonGroupTicket(NonGroupTicketInput input, int parkId, int terminalId, string tradeNo)
        {
            var entity = input.MapTo<NonGroupTicket>();
            entity.Id = await _uniqueCode.CreateAsync(CodeType.Barcode, parkId, terminalId);
            //价格数据从数据库获取赋值
            var parkSaleTicket = await _parkSaleTicketClassAppService.GetOnCacheByIdAsync(entity.ParkSaleTicketClassId);
            entity.ParkId = parkId;
            entity.TerminalId = terminalId;
            entity.TradeInfoId = tradeNo;
            entity.Price = parkSaleTicket.Price;
            entity.SalePrice = parkSaleTicket.SalePrice;
            entity.Amount = parkSaleTicket.SalePrice;
            //获取配置有效期
            entity.ValidDays = GetValidDays(parkSaleTicket.TicketClassId).Result;
            entity.ValidStartDate = entity.ValidStartDate.Date;
            entity.CreatorUserId = AbpSession.GetUserId();
            entity.TicketSaleStatus = TicketSaleStatus.Valid;

            return entity;
        }


        private async Task<OtherNonGroupTicket> InitialOtherNonGroupTicket(OtherNonGroupTicketInput input, int parkId, int terminalId, string tradeinfoId)
        {
            var entity = input.MapTo<OtherNonGroupTicket>();
            entity.Id = await _uniqueCode.CreateAsync(CodeType.Barcode, parkId, terminalId);
            //价格数据从数据库获取赋值
            var parkSaleTicket = await _parkSaleTicketClassAppService.GetOnCacheByIdAsync(entity.ParkSaleTicketClassId);
            entity.TerminalId = terminalId;
            entity.TradeInfoId = tradeinfoId;
            entity.Price = parkSaleTicket.Price;
            entity.SalePrice = parkSaleTicket.SalePrice;
            entity.Amount = parkSaleTicket.SalePrice * entity.Qty;
            entity.InparkCounts = 0;
            entity.ValidDays = GetValidDays(parkSaleTicket.TicketClassId).Result;
            entity.ValidStartDate = entity.ValidStartDate;
            entity.ParkId = parkId;
            entity.CreatorUserId = AbpSession.UserId;
            entity.TicketSaleStatus = TicketSaleStatus.Valid;

            return entity;
        }


        /// <summary>
        /// 他园票数据同步
        /// </summary>
        /// <param name="entities"></param>
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

            var result = await _parkSaleTicketClassAppService
                .GetMulSaleTicketByParkIdsAsync<ParkSaleTicketClassCacheItem>(new List<int>() { parkId }, null);
            var otherParkIds = result?.Where(p => p.Id == entity.ParkSaleTicketClassId).First().InParkIdFilter.Trim().Split(',');
            foreach (var parkid in otherParkIds)
            {
                if (parkid != parkId.ToString())
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                }
            }
        }


        /// <summary>
        /// 获取散客有效期
        /// </summary>
        /// <returns></returns>
        private async Task<int> GetValidDays(int ticketClassId)
        {
            var defaultResult = _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == null).Result ?? new DefaultPrintSet();
            var validDays = defaultResult.DefaultValidDays;
            var ticketPrintSet = await _ticketPrintSetRepository.FirstOrDefaultAsync(p => p.TicketClassId == ticketClassId);
            if (ticketPrintSet?.ValidDays != null)
                validDays = ticketPrintSet.ValidDays.Value;
            return validDays;
        }


    }
}
