using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.DataSync;
using ThemePark.Core.ParkSale;
using ThemePark.Core.TradeInfos;
using ThemePark.Core.TradeInfos.DomainServiceInterfaces;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Core;
using ThemePark.Infrastructure.EntityFramework;
using Abp.AutoMapper;
using ThemePark.Common;
using Abp.Extensions;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Application.Refund.Dto;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Enumeration;
using ThemePark.Infrastructure.Web.Api;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 退票服务层
    /// </summary>
    public class TicketRefundAppService : ThemeParkAppServiceBase, ITicketRefundAppService
    {
        #region Fields
        private readonly IUniqueCode _uniqueCode;
        private readonly ITicketRefundDomainService _ticketRefundDomainService;
        private readonly INonGroupTicketAppService _nonGroupTicketAppService;
        private readonly ITradeInfoDomainService _tradeInfoDomainService;
        private readonly IGroupTicketAppService _groupTicketAppService;
        private readonly IRepository<TradeInfo, string> _tradeInfoRepository;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IRepository<OtherNonGroupTicket, string> _otherNonGroupTicketRepository;
        private readonly IRepository<GroupTicket, string> _groupTicketRepository;
        private readonly IOtherNonGroupTicketAppService _otherNonGroupTicketAppService;
        private readonly IRepository<SyncPark> _syncParkRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Agency> _agencyRepository;
        private readonly IRepository<AccountOp, long> _accountOpRepository;
        private readonly IAgencyAccountAppService _agencyAccountAppService;
        #endregion

        #region Cotr
        /// <summary>
        /// 依赖注入
        /// </summary>
        public TicketRefundAppService(ITicketRefundDomainService ticketRefundDomainService, IGroupTicketAppService groupTicketAppService,
            INonGroupTicketAppService nonGroupTicketAppService, ITradeInfoDomainService tradeInfoDomainService, IRepository<TradeInfo, string> tradeInfoRepository,
            IDataSyncManager dataSyncManager, IUniqueCode uniqueCode, IRepository<OtherNonGroupTicket, string> otherNonGroupTicketRepository,
            IOtherNonGroupTicketAppService otherNonGroupTicketAppService, IRepository<SyncPark> syncParkRepository, IRepository<GroupTicket, string> groupTicketRepository,
            IRepository<Account> accountRepository, IRepository<Agency> agencyRepository, IRepository<AccountOp, long> accountOpRepository, IAgencyAccountAppService agencyAccountAppService)
        {
            _ticketRefundDomainService = ticketRefundDomainService;
            _nonGroupTicketAppService = nonGroupTicketAppService;
            _tradeInfoDomainService = tradeInfoDomainService;
            _groupTicketAppService = groupTicketAppService;
            _tradeInfoRepository = tradeInfoRepository;
            _dataSyncManager = dataSyncManager;
            _uniqueCode = uniqueCode;
            _otherNonGroupTicketRepository = otherNonGroupTicketRepository;
            _otherNonGroupTicketAppService = otherNonGroupTicketAppService;
            _syncParkRepository = syncParkRepository;
            _groupTicketRepository = groupTicketRepository;
            _accountRepository = accountRepository;
            _agencyRepository = agencyRepository;
            _accountOpRepository = accountOpRepository;
            _agencyAccountAppService = agencyAccountAppService;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 获取票信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<TicketRefundDetailDto> GetTicketFromGroupOrNongroup(string barcode)
        {
            //var groupTicket = _groupTicketRepository.FirstOrDefault(m => m.Id == barcode);
            var groupTicket = await _groupTicketAppService.GetTicketById(barcode);
            if (groupTicket != null)
            {
                var model = groupTicket.MapTo<TicketRefundDetailDto>();
                model.TicketCatogory = TicketCategory.GroupTicket.DisplayName();
                model.SaleTicketClassName = groupTicket.ParkSaleTicketClass.SaleTicketClassName;
                return model;
            }
            //var nonGroupTicket = _nonGroupTicketRepository.FirstOrDefault(m => m.Id == barcode);
            var nonGroupTicket = await _nonGroupTicketAppService.GetTicketById(barcode);
            if (nonGroupTicket != null)
            {
                var model = nonGroupTicket.MapTo<TicketRefundDetailDto>();
                model.TicketCatogory = TicketCategory.NonGroupTicket.DisplayName();
                model.SaleTicketClassName = nonGroupTicket.ParkSaleTicketClass.SaleTicketClassName;
                return model;
            }

            var otherNonGroupTicket = await _otherNonGroupTicketRepository.FirstOrDefaultAsync(barcode);
            if (otherNonGroupTicket != null)
            {
                var model = otherNonGroupTicket.MapTo<OtherNonGrouptTicketRefundDetailDto>();
                model.TicketCatogory = TicketCategory.OtherNonGroupTicket.DisplayName();
                model.SaleTicketClassName = otherNonGroupTicket.ParkSaleTicketClass.SaleTicketClassName;
                return model;
            }

            return null;
        }

        /// <summary>
        /// 获取票信息
        /// </summary>
        /// <param name="tradeInfoId"></param>
        /// <returns></returns>
        private async Task<List<TicketRefundDetailDto>> GetTicketFromGroupOrNongroupByTrade<T>(string tradeInfoId)
        {
            if (typeof(T).Name == typeof(GroupTicket).Name)
            {
                var ticketsTaskResult = await _groupTicketAppService.GetGroupTicketListAsync<GroupTicketRefundDetailDto>(new Query<GroupTicket>(o => o.TradeInfoId == tradeInfoId));

                var tickets = ticketsTaskResult.ForEach(o =>
                {
                    o.TicketCatogory = TicketCategory.GroupTicket.DisplayName();
                    o.FlowId = _uniqueCode.DecodeFlow(CodeType.Barcode, o.Id);
                }).ToList();

                return tickets.Cast<TicketRefundDetailDto>().ToList();
            }
            if (typeof(T).Name == typeof(NonGroupTicket).Name)
            {
                var ticketsTaskResult = await _nonGroupTicketAppService.GetNonGroupTicketListAsync<NonGroupTicketRefundDetailDto>(new Query<NonGroupTicket>(o => o.TradeInfoId == tradeInfoId));
                //var ticketList = _nonGroupTicketRepository.GetAll().AsNoTracking()
                var tickets = ticketsTaskResult.ForEach(o =>
                {
                    o.TicketCatogory = TicketCategory.NonGroupTicket.DisplayName();
                    o.FlowId = _uniqueCode.DecodeFlow(CodeType.Barcode, o.Id);
                }).ToList();

                return tickets.Cast<TicketRefundDetailDto>().ToList();
            }
            if (typeof(T).Name == nameof(OtherNonGroupTicket))
            {
                var otherNonGroupTicketEntities = _otherNonGroupTicketRepository.GetAll().Where(o => o.TradeInfoId == tradeInfoId).ToList();
                var ticketsTaskResult = otherNonGroupTicketEntities.MapTo<IList<OtherNonGrouptTicketRefundDetailDto>>();
                //var ticketList = _nonGroupTicketRepository.GetAll().AsNoTracking()
                var tickets = ticketsTaskResult.ForEach(o =>
                {
                    o.TicketCatogory = TicketCategory.OtherNonGroupTicket.DisplayName();
                    o.FlowId = _uniqueCode.DecodeFlow(CodeType.Barcode, o.Id);
                }).ToList();

                return tickets.Cast<TicketRefundDetailDto>().ToList();
            }
            return null;
        }


        public async Task<Result<IList<TicketRefundDetailDto>>> SearchTickets4RefundAsync(string barcodeBegin, string barcodeEnd)
        {
            /*todo: 退票流程 散客票，团体票
             * 区间条码 2个条码
             * 
             * 判断是否属于同一笔交易
             * 
             * 通过交易号查询出所有的条码（计算出所有条码的流水号【barcode,flowid】）
             * 
             * barcode1 流水号，barcode2 流水号
             * 
             * 取流水号之间的条码（界面数据）
             * 
             */
            int flowId1 = _uniqueCode.DecodeFlow(CodeType.Barcode, barcodeBegin);
            int flowId2 = _uniqueCode.DecodeFlow(CodeType.Barcode, barcodeEnd);

            if (flowId1 == 0 || flowId2 == 0)
            {
                return Result.FromError<IList<TicketRefundDetailDto>>("请输入正确条形码");
            }

            var ticket1 = await GetTicketFromGroupOrNongroup(barcodeBegin);
            var ticket2 = await GetTicketFromGroupOrNongroup(barcodeEnd);

            if (ticket2 == null || ticket1 == null)
            {
                return Result.FromCode<IList<TicketRefundDetailDto>>(ResultCode.NoRecord);
            }

            if (ticket1.TradeInfoId.Equals(ticket2.TradeInfoId))
            {
                int temp = flowId1;
                if (flowId1 > flowId2)
                {
                    flowId1 = flowId2;
                    flowId2 = temp;
                }

                if (ticket1.TicketCatogory.Equals(TicketCategory.NonGroupTicket.DisplayName()))
                {
                    var result = await GetTicketFromGroupOrNongroupByTrade<NonGroupTicket>(ticket1.TradeInfoId);
                    //.Where(o => o.FlowId.IsBetween(flowId1, flowId2)).ToList();

                    return Result.FromData<IList<TicketRefundDetailDto>>(result.Where(o => o.FlowId.IsBetween(flowId1, flowId2)).ToList(), null);
                }
                if (ticket1.TicketCatogory.Equals(TicketCategory.GroupTicket.DisplayName()))
                {
                    var result = await GetTicketFromGroupOrNongroupByTrade<GroupTicket>(ticket1.TradeInfoId);

                    return Result.FromData<IList<TicketRefundDetailDto>>(result.Where(o => o.FlowId.IsBetween(flowId1, flowId2)).ToList(), null);
                }
                if (ticket1.TicketCatogory.Equals(TicketCategory.OtherNonGroupTicket.DisplayName()))
                {
                    var result = await GetTicketFromGroupOrNongroupByTrade<OtherNonGroupTicket>(ticket1.TradeInfoId);

                    return Result.FromData<IList<TicketRefundDetailDto>>(result.Where(o => o.FlowId.IsBetween(flowId1, flowId2)).ToList(), null);
                }
            }
            return Result.FromError<IList<TicketRefundDetailDto>>("首尾条码不属于同一笔交易");
        }

        /// <summary>
        /// 根据条码查询取票信息
        /// </summary>
        /// <param name="barcode">条码</param>
        /// <returns></returns>
        public async Task<Result<TicketRefundDetailDto>> SearchTicket4RefundAsync(string barcode)
        {
            var flowId = _uniqueCode.DecodeFlow(CodeType.Barcode, barcode);
            if (flowId <= 0)
                return Result.FromError<TicketRefundDetailDto>("请输入正确条形码");
            else
            {
                var ticket = await GetTicketFromGroupOrNongroup(barcode);
                if (ticket == null)
                    return Result.FromCode<TicketRefundDetailDto>(ResultCode.NoRecord);
                else
                {
                    return Result.FromData<TicketRefundDetailDto>(ticket);
                }
            }
        }

        /// <summary>
        /// 新增退票记录（已出票和已入园的不能退款）（有效的且没入过园的票才能退款）
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddTicketRefundAsync(List<RefundTicketInput> inputs, int terminalId, int parkId)
        {
            if (inputs.Count == 0)
                return Result.FromError("退票列表不能为空");

            Result<PayType> isInvalidOrContainPayType = new Result<PayType>();
            //1. 业务验证
            foreach (var input in inputs)
            {
                //检验是否他园票、过期或者已入园
                isInvalidOrContainPayType = await CheckIfTicketIsExpireOrInvalid(input.BarCode, input.TicketCatogory);

                if (!isInvalidOrContainPayType.Success)
                    return isInvalidOrContainPayType;
            }

            //总额
            var totalAmount = inputs.Sum(o => o.Amount);
            var tradeInfo = new TradeInfo
            {
                Amount = totalAmount,
                TradeType = TradeType.Outlay,
                TradeInfoDetails = new List<TradeInfoDetail>
                {
                    new TradeInfoDetail
                    {
                        Amount = totalAmount,
                        PayModeId = isInvalidOrContainPayType.Data,
                        PayStatus = PayStatus.PaySuccess
                    }
                },
                PayModeId = isInvalidOrContainPayType.Data
            };

            //2. 为所有退款的票添加 一条交易记录
            var tradeInfoNew = await _tradeInfoDomainService.AddTradeInfo4RefundAsync(tradeInfo, parkId);

            //2. 如果是预付款和挂账需要把退票金额返回到账户
            if (isInvalidOrContainPayType.Data == PayType.PrePay || isInvalidOrContainPayType.Data == PayType.Account)
            {
                var entity = await _groupTicketRepository.FirstOrDefaultAsync(inputs.First().BarCode);
                var agency = _agencyRepository.FirstOrDefault(m => m.Id == entity.AgencyId);
                var accountOp = new AccountOpInput
                {
                    AccountId = agency.AccountId,
                    Cash = totalAmount,
                    //退款
                    OpType = OpType.Refund,
                    TradeInfoId = tradeInfoNew.Id
                };
                await _agencyAccountAppService.RefundAccount(accountOp, parkId);
            }

            //3. 更改票状态为作废，订单状态为已退款
            foreach (var input in inputs)
            {
                var updateResult = await UpdateTicketsToInvalidAsync(input.BarCode, input.TicketClassMode, parkId, input.TicketCatogory);

                if (!updateResult.Success)
                    return updateResult;

                //4. 清掉验票的缓存
                var ticketCheckCacheDto = new TicketCheckCacheDto
                {
                    Key = input.BarCode
                };

                DataSyncInput dataSyncInput = new DataSyncInput()
                {
                    SyncData = JsonConvert.SerializeObject(ticketCheckCacheDto),
                    SyncType = DataSyncType.TicketCheckCacheClear
                };
                _dataSyncManager.UploadDataToTargetPark(parkId, dataSyncInput);

                //5. 添加退票记录
                var entity = new TicketRefund()
                {
                    OriginalTradeInfoId = updateResult.Data,
                    Id = input.BarCode,
                    TerminalId = terminalId,
                    Amount = input.Amount,
                    TradeInfoId = tradeInfoNew.Id,

                };
                await _ticketRefundDomainService.AddTicketRefundAndReturnIdAsync(entity);
            }

            return Result.Ok();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 更新散客票/团体票/网络票状态为退款
        /// </summary>
        private async Task<Result<string>> UpdateTicketsToInvalidAsync(string barCode, TicketClassMode ticketClassMode, int parkId, string ticketCatogory)
        {
            if (!string.IsNullOrWhiteSpace(barCode))
            {
                Result<string> result;
                switch (ticketCatogory)
                {
                    case "散客票":
                        result = await UpdateNonGroupTicket(barCode, ticketClassMode, parkId);
                        break;
                    case "团体票":
                        result = await UpdateGroupTicket(barCode, ticketClassMode, parkId);
                        break;
                    case "他园票":
                        result = await UpdateOhterNonGroupTicket(barCode);
                        break;
                    default:
                        result = Result.FromError<string>("不存在该票类");
                        break;
                }
                return result;
            }

            return Result.FromError<string>("条形码不能为空");
        }

        /// <summary>
        /// 检验票是否符合退票规则
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="ticketCatogory"></param>
        /// <returns>false为在有效期内，true为过期</returns>
        private async Task<Result<PayType>> CheckIfTicketIsExpireOrInvalid(string barCode, string ticketCatogory)
        {
            Result<PayType> result;
            switch (ticketCatogory)
            {
                case "散客票":
                    result = await VerifyNonGroupTicketRefundBusiness(barCode);
                    break;
                case "团体票":
                    result = await VerifyGroupTicketRefundBusiness(barCode);
                    break;
                case "他园票":
                    result = await VerifyOtherNonGroupTicketRefundBusiness(barCode);
                    break;
                default:
                    result = Result.FromError<PayType>("没有该票类");
                    break;
            }
            return result;
        }

        /// <summary>
        /// 散客票退票业务验证
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<PayType>> VerifyNonGroupTicketRefundBusiness(string barcode)
        {
            var nonGroupTicket =
                await _nonGroupTicketAppService.GetTicketById(barcode);

            //检验是否是他园票，他园票不允许退
            if (nonGroupTicket.ParkId != AbpSession.LocalParkId)
                return Result.FromError<PayType>("非本公园售票不允许退票");

            //包括了已入园，已过期，已冻结，已作废
            if (!(nonGroupTicket.TicketSaleStatus == TicketSaleStatus.Valid && nonGroupTicket.InparkCounts == 0))
                return Result.FromError<PayType>("票不允许退票");

            //现金支付才能退款
            var tradeInfo = await _tradeInfoRepository.AsNoTrackingAndInclude(m => m.TradeInfoDetails).FirstOrDefaultAsync(m => m.Id == nonGroupTicket.TradeInfoId);

            var payType = tradeInfo.TradeInfoDetails.Select(m => m.PayModeId).ToList();
            if (payType.Count > 1)
                return Result.FromError<PayType>("多种支付方式交易的票不允许退款");

            //只有全部是现金交易的才能退款
            //2017-9-29需求更改 只有多种支付方式不能退，其他都按原有方式退
            //var tradeResult = tradeInfo.TradeInfoDetails.All(m => m.PayModeId == PayType.Cash || m.PayModeId == PayType.Account
            //|| m.PayModeId == PayType.PrePay || m.PayModeId == PayType.BankCard);

            //if (!tradeResult)
            //    return Result.FromError<PayType>("交易方式为" + payType + "不允许退款");

            return Result.FromData(payType.First());
        }

        /// <summary>
        /// 团体票退票业务验证
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<PayType>> VerifyGroupTicketRefundBusiness(string barcode)
        {
            var groupTicket =
                await _groupTicketAppService.GetTicketById(barcode);
            //检验是否是他园票，他园票不允许退
            if (groupTicket.ParkId != AbpSession.LocalParkId)
                return Result.FromError<PayType>("非本公园售票不允许退票");

            if (!(groupTicket.TicketSaleStatus == TicketSaleStatus.Valid && groupTicket.InparkCounts == 0))
                return Result.FromError<PayType>("票不允许退票");

            //现金支付才能退款
            var tradeInfo = await _tradeInfoRepository.AsNoTrackingAndInclude(m => m.TradeInfoDetails).FirstOrDefaultAsync(m => m.Id == groupTicket.TradeInfoId);

            var payType = tradeInfo.TradeInfoDetails.Select(m => m.PayModeId).ToList();
            if (payType.Count > 1)
                return Result.FromError<PayType>("多种支付方式交易的票不允许退款");

            //只有全部是现金交易的才能退款
            //2017-9-29需求更改 只有多种支付方式不能退，其他都按原有方式退
            //var tradeResult = tradeInfo.TradeInfoDetails.All(m => m.PayModeId == PayType.Cash || m.PayModeId == PayType.Account
            //|| m.PayModeId == PayType.PrePay || m.PayModeId == PayType.BankCard);

            //if (!tradeResult)
            //    return Result.FromError<PayType>("交易方式为" + payType + "不允许退款");

            return Result.FromData(payType.First());
        }

        /// <summary>
        /// 他园票退票业务验证
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<PayType>> VerifyOtherNonGroupTicketRefundBusiness(string barcode)
        {
            var otherNonGroupTicket =
                await _otherNonGroupTicketRepository.FirstOrDefaultAsync(barcode);

            if (otherNonGroupTicket == null)
                return Result.FromError<PayType>("非本公园售票不允许退票");

            //公园同步接口
            var syncParkId = otherNonGroupTicket.ParkId;
            var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == syncParkId);
            var uri = new Uri(sync.SyncUrl);

            var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/TicketBusiness/OtherNonGroupTicketRefundAsync?barCode=" + barcode, string.Empty);

            ////请求本地api(测试用)
            //var baseAddress = "http://localhost:59029";
            //var response = await HttpHelper.PostAsync(baseAddress, "/Api/OrderDetail/OtherNonGroupTicketRefundAsync?barCode=" + barcode, string.Empty);

            var result = JsonConvert.DeserializeObject<Result>(response);

            if (!result.Success)
                return Result.FromError<PayType>(result.Message);

            //现金支付才能退款
            var tradeInfo = await _tradeInfoRepository.AsNoTrackingAndInclude(m => m.TradeInfoDetails).FirstOrDefaultAsync(m => m.Id == otherNonGroupTicket.TradeInfoId);

            var payType = tradeInfo.TradeInfoDetails.Select(m => m.PayModeId).ToList();
            if (payType.Count > 1)
                return Result.FromError<PayType>("多种支付方式交易的票不允许退款");

            //只有全部是现金交易的才能退款
            //2017-9-29需求更改 只有多种支付方式不能退，其他都按原有方式退
            //var tradeResult = tradeInfo.TradeInfoDetails.All(m => m.PayModeId == PayType.Cash || m.PayModeId == PayType.Account
            //|| m.PayModeId == PayType.PrePay || m.PayModeId == PayType.BankCard);

            //if (!tradeResult)
            //    return Result.FromError<PayType>("交易方式为" + payType + "不允许退款");

            return Result.FromData(payType.First());
        }


        /// <summary>
        /// 更新散客票状态和同步到公园
        /// </summary>
        /// <returns></returns>
        private async Task<Result<string>> UpdateNonGroupTicket(string barCode, TicketClassMode ticketClassMode, int parkId)
        {
            //1. 更新散客票状态为无效
            var nonGroupResult = await _nonGroupTicketAppService.UpdateNonGroupTicketToInvalidAndReturnOriginalTradeIdAsync(barCode);
            if (nonGroupResult.Success)
            {
                if (ticketClassMode == TicketClassMode.MultiParkTicket)
                {
                    //2. 退票 套票同步
                    var nonGroupTicket =
                        await _nonGroupTicketAppService.GetNonGroupTicketAsync<NonGroupTicketDto>(
                            new Query<NonGroupTicket>(o => o.Id == barCode));

                    var otherParkIds = nonGroupTicket.ParkSaleTicketClass.TicketClass.InParkIdFilter.Trim().Split(',');
                    foreach (var parkid in otherParkIds)
                    {
                        if (parkid != parkId.ToString())
                        {
                            var syncParkId = Convert.ToInt32(parkid);
                            //公园同步接口
                            var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == syncParkId);
                            var uri = new Uri(sync.SyncUrl);

                            var syncModel = new MultiTicketCancelDto
                            {
                                Barcode = barCode,
                                TicketCategory = TicketCategory.NonGroupTicket
                            };
                            var requestUrl =
                                $"/{ApiRouteConstant.ApiPrefix}{ApiRouteConstant.TicketBusinessRoute}/{ApiRouteConstant.MultiTicketCancelRoute}";

                            var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), requestUrl, JsonConvert.SerializeObject(syncModel));

                            var result = JsonConvert.DeserializeObject<Result>(response);

                            if (!result.Success)
                                return Result.FromError<string>(result.Message);
                        }
                    }
                }
            }
            return nonGroupResult;
        }

        /// <summary>
        /// 更新团体票状态和同步到公园
        /// </summary>
        /// <returns></returns>
        private async Task<Result<string>> UpdateGroupTicket(string barCode, TicketClassMode ticketClassMode, int parkId)
        {
            //1. 更新团体票状态为无效
            var entity = await _groupTicketRepository.FirstOrDefaultAsync(m => m.Id == barCode);

            if (entity != null)
            {
                entity.TicketSaleStatus = TicketSaleStatus.Refund;
            }
            else
                return Result.FromError<string>(ResultCode.NoRecord.DisplayName());


            //3. 退票 套票同步
            if (ticketClassMode == TicketClassMode.MultiParkTicket)
            {
                var groupTicket =
                    await _groupTicketAppService.GetGroupTicketAsync<GroupTicketDto>(
                        new Query<GroupTicket>(o => o.Id == barCode));


                var otherParkIds = groupTicket.ParkSaleTicketClass.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        var syncParkId = Convert.ToInt32(parkid);
                        //公园同步接口
                        var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == syncParkId);
                        var uri = new Uri(sync.SyncUrl);

                        var syncModel = new MultiTicketCancelDto
                        {
                            Barcode = barCode,
                            TicketCategory = TicketCategory.GroupTicket
                        };
                        var requestUrl =
                                $"/{ApiRouteConstant.ApiPrefix}{ApiRouteConstant.TicketBusinessRoute}/{ApiRouteConstant.MultiTicketCancelRoute}";
                        var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), requestUrl, JsonConvert.SerializeObject(syncModel));

                        var result = JsonConvert.DeserializeObject<Result>(response);

                        if (!result.Success)
                            return Result.FromError<string>(result.Message);
                    }
                }
            }

            return Result.FromData(entity.TradeInfoId);
        }

        /// <summary>
        /// 更新他园票状态和同步到公园
        /// </summary>
        /// <returns></returns>
        private async Task<Result<string>> UpdateOhterNonGroupTicket(string barCode)
        {
            //1. 更新他园票状态为无效
            var otherNonGroupTicketResult = await _otherNonGroupTicketAppService.UpdateGroupTicketToInvalidAndReturnTradeIdAsync(barCode);

            if (otherNonGroupTicketResult.Success)
            {
                //2. 退票 套票同步
                var syncModel = new MultiTicketCancelDto { Barcode = barCode, TicketCategory = TicketCategory.OtherNonGroupTicket };

                var otherNonGroupTicket =
                    _otherNonGroupTicketRepository.FirstOrDefault(barCode).MapTo<OtherNonGroupTicketDto>();

                //公园同步接口
                var syncParkId = otherNonGroupTicket.ParkId;
                var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == syncParkId);
                var uri = new Uri(sync.SyncUrl);

                var requestUrl =
                                $"/{ApiRouteConstant.ApiPrefix}{ApiRouteConstant.TicketBusinessRoute}/{ApiRouteConstant.MultiTicketCancelRoute}";
                var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), requestUrl, JsonConvert.SerializeObject(syncModel));

                var result = JsonConvert.DeserializeObject<Result>(response);

                if (!result.Success)
                    return Result.FromError<string>(result.Message);
            }

            return otherNonGroupTicketResult;
        }

        #endregion
    }
}
