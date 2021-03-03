using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using System.Data.Entity;
using Abp.Domain.Uow;
using Newtonsoft.Json;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.Message;
using ThemePark.Application.OTA;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket.Repositories;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.DataSync;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Web.Api;
using ThemePark.Application.DataSync;
using Abp.BackgroundJobs;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 订单应用服务
    /// </summary>
    public class TOHeaderAppService : ThemeParkAppServiceBase, ITOHeaderAppService
    {
        #region Fields

        private readonly ITOHeaderRepository _tOHeaderRepository;
        private readonly IRepository<TOHeaderPre, string> _tOHeaderPreRepository;
        private readonly IRepository<TOBodyPre, string> _tOBodyPreRepository;
        private readonly IRepository<TOBody, string> _tOBodyRepository;
        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketClassRepository;
        private readonly IRepository<TicketClass> _ticketClassRepository;
        private readonly IRepository<ParkAgencyTypeGroupType> _parkAgencyTypeGroupTypeRepository;
        private readonly IOTADataSync _otaDataSync;
        private readonly IAgencyAppService _agencyAppService;
        private readonly ISmsAppService _smsAppService;
        private readonly IRepository<TOTicket, string> _toTicketRepository;
        private readonly IRepository<GroupInfo, long> _groupInfoRepository;
        private readonly ICheckTicketManager _checkTicketManager;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IRepository<SyncPark> _syncParkRepositoty;
        #endregion

        #region Ctor
        /// <summary>
        /// 构造
        /// </summary>
        public TOHeaderAppService(IRepository<TOBody, string> tOBodyRepository, IRepository<AgencySaleTicketClass> agencySaleTicketClassRepository,
            IRepository<TicketClass> ticketClassRepository, IRepository<TOHeaderPre, string> tOHeaderPreRepository,
            ITOHeaderRepository tOHeaderRepository, IAgencyAppService agencyAppService, IRepository<SyncPark> syncParkRepositoty,
            IRepository<ParkAgencyTypeGroupType> parkAgencyTypeGroupTypeRepository, IRepository<TOBodyPre, string> tOBodyPreRepository,
            IOTADataSync otaDataSync, ICheckTicketManager checkTicketManager,
            IRepository<GroupInfo, long> groupInfoRepository, IRepository<TOTicket, string> toTicketRepository, ISmsAppService smsAppService,
            IDataSyncManager dataSyncManager)
        {
            _tOBodyRepository = tOBodyRepository;
            _agencySaleTicketClassRepository = agencySaleTicketClassRepository;
            _ticketClassRepository = ticketClassRepository;
            _checkTicketManager = checkTicketManager;
            _groupInfoRepository = groupInfoRepository;
            _toTicketRepository = toTicketRepository;
            _smsAppService = smsAppService;
            _tOHeaderRepository = tOHeaderRepository;
            _parkAgencyTypeGroupTypeRepository = parkAgencyTypeGroupTypeRepository;
            _otaDataSync = otaDataSync;
            _agencyAppService = agencyAppService;
            _toTicketRepository = toTicketRepository;
            _tOHeaderPreRepository = tOHeaderPreRepository;
            _tOBodyPreRepository = tOBodyPreRepository;
            _dataSyncManager = dataSyncManager;
            _syncParkRepositoty = syncParkRepositoty;
        }

        #endregion

        #region Public Methods        

        /// <summary>
        /// 确认订单（后台更改订单后的确认）
        /// </summary>
        public async Task<Result> EditAndConfirmOrderAsync(ConfirmOrderInput input)
        {
            /*
             *  1. 确认前确保这条订单不会在别的电脑已经确认了
             *  2. 生成确认订单（TOHeader,TOBody）,TOHeader的id需要和主预订单主键一样
             *  3. 修改预订单确认状态
             *  4. 同步确认的订单
             */

            var toHeader = await _tOHeaderRepository.GetAllIncluding(p => p.Agency).FirstOrDefaultAsync(p => p.Id == input.Id);

            //检测公园是否已经取票了或者已经在别的电脑被确认
            if (toHeader != null)
                return Result.FromCode(ResultCode.InvalidData, "该订单已经被确认，请勿重复确认");

            //业务验证
            var result = await VerifyBusinessRuleForCentreUpdateOrder(input);
            if (!result.Success)
                return result;

            //更新预订主订单和预订子订单确认票数和确认人数
            var toheaderPre = await UpdateTOHeaderPreAndTOBodyPre(input);

            var toheader = new TOHeader
            {
                AgencyId = toheaderPre.AgencyId,
                Amount = input.Amount,
                GroupInfoId = toheaderPre.GroupInfoId,
                Id = toheaderPre.Id,
                Persons = input.Persons,
                Qty = input.Qty,
                OrderType = OrderType.TravelOrder,
                GroupTypeId = toheaderPre.GroupTypeId,
                ValidStartDate = toheaderPre.ValidStartDate,
                Remark = input.ToHeadRemark
            };

            await _groupInfoRepository.UpdateAsync(toheaderPre.GroupInfoId.Value,
                p => Task.WhenAll(Task.FromResult(p.DriverIds = input.GroupInfo.DriverIds),
                Task.FromResult(p.GuideIds = input.GroupInfo.GuideIds),
                Task.FromResult(p.LicensePlateNumber = input.GroupInfo.LicensePlateNumber)));

            //await _groupInfoRepository.UpdateAsync(toheaderPre.GroupInfoId.Value,
            //   p => Task.FromResult(p.GuideIds = input.GroupInfo.GuideIds));

            //新生成确认后的主订单
            var id = await _tOHeaderRepository.InsertAndGetIdAsync(toheader);

            //新生成确认后的子订单
            AddNewTOBody(input);

            //更新代理商电话
            await _agencyAppService.UpdateAgencyPhoneAsync(toheaderPre.AgencyId, input.Phone);

            //确保确认订单提交到数据库，后面需要从数据库查询这个确认订单
            await CurrentUnitOfWork.SaveChangesAsync();

            //重新查询新的主订单，因为子订单已经变更            
            var tohead = await _tOHeaderRepository.GetAllIncluding(p => p.TOBodies.Select(m => m.AgencySaleTicketClass), p => p.GroupType, p => p.GroupInfo, p => p.Agency).FirstAsync(p => p.Id == id);
            //数据同步
            DataSyncToPark(tohead);

            await _smsAppService.SendTravelMessage(tohead);

            return Result.Ok();
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<Result> ConfirmOrderAsync(string id)
        {
            /*
             *  1. 生成确认的主订单和子订单
             *  2. 更改预订主订单和预订子订单的数量和状态
             *  3. 发送短信
             *  4. 数据同步
             * 
             */
            var toHeaderPre = await _tOHeaderPreRepository.GetAllIncluding(m => m.TOBodyPres).FirstOrDefaultAsync(p => p.Id == id);

            if (toHeaderPre == null)
                return Result.FromError("没有该订单");

            //检测公园是否已经取票了或者已经在别的电脑被确认
            if (toHeaderPre.MainOrderState != MainOrderState.NotConfirm)
                return Result.FromError("订单不是未确认状态，不允许确认。");

            //新生成的确认订单的数量和人数是包括确免和预免的数量，因为数量只有一个所以要存所有的数量
            var toheader = new TOHeader
            {
                AgencyId = toHeaderPre.AgencyId,
                Amount = toHeaderPre.Amount,
                GroupInfoId = toHeaderPre.GroupInfoId,
                Id = toHeaderPre.Id,
                Persons = toHeaderPre.Persons + toHeaderPre.FreePersons,
                Qty = toHeaderPre.Qty + toHeaderPre.FreeQty,
                OrderType = OrderType.TravelOrder,
                GroupTypeId = toHeaderPre.GroupTypeId,
                ValidStartDate = toHeaderPre.ValidStartDate,
                Remark = toHeaderPre.Remark
            };

            //新生成确认后的主订单
            await _tOHeaderRepository.InsertAndGetIdAsync(toheader);

            //新生成确认后的子订单
            ConvertToTOBody(toHeaderPre.TOBodyPres);

            //更新预订主订单和预订子订单
            UpdateConfirmTOHeaderPreAndTOBodyPre(toHeaderPre);

            //确保确认订单提交到数据库，后面需要从数据库查询这个确认订单
            await CurrentUnitOfWork.SaveChangesAsync();

            //重新查询新的主订单，因为子订单已经变更            
            var tohead = await _tOHeaderRepository.GetAllIncluding(p => p.TOBodies.Select(m => m.AgencySaleTicketClass), p => p.GroupType, p => p.GroupInfo, p => p.Agency).FirstAsync(p => p.Id == id);
            //数据同步
            DataSyncToPark(tohead);

            //发短信
            await _smsAppService.SendTravelMessage(tohead);

            return Result.Ok();
        }

        /// <summary>
        /// 重新发送短信服务
        /// </summary>
        /// <returns></returns>
        public async Task<Result> ResendMessage(string id)
        {
            var tohead = await _tOHeaderRepository.GetAllIncluding(p => p.TOBodies.Select(m => m.AgencySaleTicketClass), p => p.GroupType, p => p.GroupInfo, p => p.Agency).FirstAsync(p => p.Id == id);
            return await _smsAppService.SendTravelMessage(tohead);
        }

        /// <summary>
        /// 中心取消确认的旅行社订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> CancelConfirmOrderAsync(string id)
        {
            /*
             *  1. 确保确认过的订单取消前不会已经被消费或退款
             *  2. 更改预订主订单和预订子订单状态
             *  3. 同步（公园也删除取消确认的订单）
             *  4. 删除中心的确认订单（TOHeader,TOBody）
             */

            var toHeaderPre = await _tOHeaderPreRepository.GetAsync(id);

            //确保取消前这个已确认的确认订单存在(已经取消确认的不能再取消)
            var toHeader = await _tOHeaderRepository.FirstOrDefaultAsync(id);
            if (toHeader == null || toHeaderPre.MainOrderState != MainOrderState.Confirm)
                return Result.FromError<TOHeader>("该订单不是已确认状态，不允许取消确认");

            if (toHeader.TOBodies.Any(m => m.OrderState == OrderState.TradeSuccess || m.OrderState == OrderState.OrderRefunded))
                return Result.FromError<TOHeader>("此订单已经被消费或退款");

            //更改预订主订单和子订单
            UpdateCancelTOHeaderPreAndTOBodyPre(toHeaderPre);

            //同步到公园
            var defaultParkId = toHeader.TOBodies.Select(o => o.ParkId).First();

            var cancelOrderConfirmDto = new OrderCancelConfirmDto
            {
                ParkId = defaultParkId,
                TOHeaderId = toHeader.Id
            };
            var syncUrl = _syncParkRepositoty.GetAll().First(m => m.ParkId == defaultParkId);
            var uri = new Uri(syncUrl.SyncUrl);

            var requestUrl =
                $"/{ApiRouteConstant.ApiPrefix}{ApiRouteConstant.OrderBusinessRoute}/{ApiRouteConstant.OrderCancelConfirmRoute}";
            //测试用
            //var baseAddress = "http://localhost:59029";
            //var jsonResult = await HttpHelper.PostAsync(baseAddress, requestUrl, JsonConvert.SerializeObject(cancelOrderConfirmDto));

            //改用同步接口保证数据冥等
            //同步到公园，通知改订单已被取消确认（公园会删除已经生成的订单信息DataSyncOrderAppService.OrderConfirmModify）
            var jsonResult = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), requestUrl, JsonConvert.SerializeObject(cancelOrderConfirmDto));
            var result = JsonConvert.DeserializeObject<Result>(jsonResult);
            if (result.Success)
            {
                //存在则删除
                if (_tOBodyRepository.GetAll().Any(m => m.TOHeaderId == toHeader.Id))
                {
                    //硬删除中心的主订单和子订单已确认记录
                    using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                    {
                        await _tOBodyRepository.DeleteAsync(m => m.TOHeaderId == toHeader.Id);
                        await _tOHeaderRepository.DeleteAsync(toHeader);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 取消OTA订单，即退款
        /// </summary>
        /// <param name="headerId">The header identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> OrderRefundAsync(string headerId)
        {
            var bodyIds = await _tOBodyRepository.GetAll().AsNoTracking().Where(o => o.TOHeaderId == headerId)
                .Select(o => o.Id).ToListAsync();

            //1. 业务验证
            if (bodyIds.Count == 0)
                return Result.FromError("该订单不存在");

            //2. 所有子订单为未消费方可退款
            if (!await _tOBodyRepository.GetAll().Where(o => bodyIds.Contains(o.Id)).AllAsync(o => o.OrderState == OrderState.WaitCost))
            {
                return Result.FromCode(ResultCode.Fail);
            }

            var ticketIds = await _toTicketRepository.GetAll().Where(o => bodyIds.Contains(o.TOVoucher.TOBodyId))
                .Select(o => o.Id).ToListAsync();

            //3. 防止线上极速退票现象
            foreach (var ticket in ticketIds)
            {
                var ticketCheck = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(ticket);
                if (ticketCheck != null && ticketCheck.TicketSaleStatus != TicketSaleStatus.Valid)
                {
                    return Result.FromCode(ResultCode.Fail);
                }
            }

            //4. 票状态更改为已退款
            foreach (var bodyId in bodyIds)
            {
                await _tOBodyRepository.UpdateAsync(bodyId, p => Task.FromResult(p.OrderState = OrderState.OrderRefunded));
            }

            foreach (var ticketId in ticketIds)
            {
                //清掉验票的缓存
                var ticketCheckCacheDto = new TicketCheckCacheDto
                {
                    Key = ticketId
                };
                var parkId = await _toTicketRepository.GetAll().Where(m => m.Id == ticketId).Select(m => m.ParkId).FirstAsync();
                DataSyncInput dataSyncInput = new DataSyncInput()
                {
                    SyncData = JsonConvert.SerializeObject(ticketCheckCacheDto),
                    SyncType = DataSyncType.TicketCheckCacheClear
                };

                this._dataSyncManager.UploadDataToTargetPark(parkId, dataSyncInput);
                await _toTicketRepository.UpdateAsync(ticketId, ticket => Task.FromResult(ticket.TicketSaleStatus = TicketSaleStatus.Refund));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 搜索旅行社订单信息列表
        /// </summary>
        public async Task<PageResult<TDto>> GetTravelOrdersAsync<TDto>(IPageQuery<TOHeader> query = null)
        {
            query = query ?? new PageQuery<TOHeader>();
            var expression = query.GetFilter().And(p => p.OrderType == OrderType.TravelOrder);
            var searchQuery = new PageQuery<TOHeader>(expression)
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortFields = query.SortFields
            };

            var result = _tOHeaderRepository.GetAll().Where(searchQuery);

            return await result.ToPageResultAsync<TOHeader, TDto>(new Query<TOHeader>(), query);
        }

        /// <summary>
        /// 搜索OTA订单信息列表(由于这个页面有特殊需求，需要单独根据子订单状态作为查询条件，所以需要根据子订单状态过滤出主订单)
        /// </summary>
        public async Task<PageResult<TDto>> GetPagedOtaOrdersAsync<TDto>(OtaSearchOrderDto query = null)
        {
            var permissionParks = AbpSession.Parks;
            query = query ?? new OtaSearchOrderDto();

            //解决前端选择一样时间时没有数据问题
            if (query.CreationTimeBegin != null && query.CreationTimeEnd != null && query.CreationTimeBegin.Value == query.CreationTimeEnd.Value)
            {
                query.CreationTimeEnd = query.CreationTimeEnd.Value.AddDays(1);
            }
            var expression = query.GetFilter().And(p => p.OrderType == OrderType.OTAOrder);

            //根据游客身份证/手机进行过滤，只要该笔订单(单个子订单)包含该用户，将显示出来
            if (!string.IsNullOrWhiteSpace(query.Pid))
                expression = expression.And(p => p.TOBodies.Any(o => o.Customer.Pid == query.Pid));
            if (!string.IsNullOrWhiteSpace(query.Phone))
                expression = expression.And(p => p.TOBodies.Any(o => o.Customer.PhoneNumber == query.Phone));

            var searchQuery = new PageQuery<TOHeader>(expression)
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortFields = query.SortFields
            };

            var result = _tOHeaderRepository.GetAllIncluding(m => m.TOBodies).Where(searchQuery);

            //筛选出不在权限范围内的主订单id
            result = result.Where(m => m.TOBodies.Count != 0);

            //根据子订单状态过滤，只要有一个子订单不符合，将不显示这整笔主订单，这是考虑到冻结功能是根据整笔订单操作的场景
            if (query.State != null)
            {
                var stateResult = result;
                //筛选出不符合子订单条件的主订单id
                var stateOrderIds =
                    stateResult.Where(
                        m =>
                            m.TOBodies.Any(n => n.OrderState != query.State)).Select(a => a.Id);
                result = result.Where(p => !stateOrderIds.Contains(p.Id));

                //result = result.Where(m => m.TOBodies.Any(n => n.OrderState != query.State));
            }

            //var bodyResult = result;
            ////筛选出不在权限范围内的主订单id
            //var orderIds =
            //    bodyResult.Where(
            //        m =>
            //            !m.TOBodies.Any(n => permissionParks.Contains(n.ParkId))).Select(a => a.Id).ToList();

            //result = result.Where(p => !orderIds.Contains(p.Id));

            return await result.ToPageResultAsync<TOHeader, TDto>(new Query<TOHeader>(), query);
        }

        /// <summary>
        /// 根据条件查找TOHeader
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetToHeaderAsync<TDto>(IQuery<TOHeader> query)
        {
            return await _tOHeaderRepository.GetAll().FirstOrDefaultAsync<TOHeader, TDto>(query);
        }

        /// <summary>
        /// 获取TOHeader表最新订单号TOHeaderId
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetLastToHeaderIdAsync()
        {
            var data = await _tOHeaderRepository.GetAll().OrderByDescending(m => m.CreationTime).Select(m => m.Id).FirstOrDefaultAsync();
            return data;
        }

        /// <summary>
        /// 根据条件查找TOHeader 列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<TDto>> GetToHeadListAsync<TDto>(IQuery<TOHeader> query)
        {
            return await _tOHeaderRepository.GetAll().ToListAsync<TOHeader, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取所有子订单
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<TDto>> GetToBodyListAsync<TDto>(IQuery<TOBody> query)
        {
            return await _tOBodyRepository.GetAll().ToListAsync<TOBody, TDto>(query);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 中心订单更改后业务验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Result> VerifyBusinessRuleForCentreUpdateOrder(ConfirmOrderInput input)
        {
            if (input.TOBodyPres.Count == 0)
                return Result.FromCode<string>(ResultCode.InvalidData, "至少要选择一种票");

            int totalQty = 0;

            foreach (var toBody in input.TOBodyPres)
            {
                //查询该促销票
                var agencySaleTicketClass = _agencySaleTicketClassRepository.AsNoTracking().FirstOrDefault(o => o.Id == toBody.AgencySaleTicketClassId && o.Status == TicketClassStatus.Sailing);
                if (agencySaleTicketClass == null)
                    return Result.FromCode<string>(ResultCode.InvalidData, "该促销票类不存在或已下架");

                //查询基础票类
                var ticketClass = await _ticketClassRepository.FirstOrDefaultAsync(agencySaleTicketClass.ParkSaleTicketClass.TicketClassId);

                //子订单
                toBody.Price = agencySaleTicketClass.Price;
                toBody.SalePrice = agencySaleTicketClass.SalePrice;
                toBody.SettlementPrice = agencySaleTicketClass.SettlementPrice;
                toBody.Amount = agencySaleTicketClass.SalePrice * toBody.Qty;
                toBody.Persons = ticketClass.TicketType.Persons * toBody.Qty;

                //主订单
                input.Amount += toBody.Amount;
                input.Qty += toBody.Qty;
                input.Persons += toBody.Persons;

                //基础票类的价格为0且包含司机和导游的（司机票和导游票），不算入代理商规则人数里面
                //2017.12.26 业务变更，促销价为0的不管是什么票都不算入代理商验证规则
                if (ticketClass.StandardPrice == 0)
                    continue;

                //用来验证代理商规则人数(导游票和司机票不包含在里面)
                totalQty += toBody.Qty;
            }

            //检测是否满足代理商规则
            var result = await CheckIfMatchAgencyRule(input.ParkAgencyTypeGroupTypeId, totalQty);
            if (!result.Success)
                return result;

            return Result.FromCode<string>(ResultCode.Ok);
        }

        /// <summary>
        /// 检测是否满足代理商规则
        /// </summary>
        /// <param name="parkAgencyTypeGroupTypeId"></param>
        /// <param name="totalQty"></param>
        /// <returns></returns>
        private async Task<Result> CheckIfMatchAgencyRule(int parkAgencyTypeGroupTypeId, int totalQty)
        {
            var qtySum = await _parkAgencyTypeGroupTypeRepository.AsNoTrackingAndInclude(m => m.AgencyRule)
               .FirstOrDefaultAsync(m => m.Id == parkAgencyTypeGroupTypeId);

            if (totalQty < qtySum.AgencyRule.MinQty || totalQty > qtySum.AgencyRule.MaxQty)
            {
                return Result.FromError<string>($"预订人数必须满足{qtySum.AgencyRule.MinQty}到{qtySum.AgencyRule.MaxQty}之间");
            }
            return Result.Ok();
        }

        /// <summary>
        /// 中心确认订单后需要同步订单数据到公园
        /// </summary>
        /// <param name="toheader"></param>
        private void DataSyncToPark(TOHeader toheader)
        {
            //中心确认订单后需要同步订单数据到公园
            var orderSend = new OrderSendDto()
            {
                OrderInfo = toheader.MapTo<SendTOHeader>(),
                TicketsInfo = new List<SendTOTicket>()
            };

            foreach (var parkId in toheader.TOBodies.Select(o => o.ParkId).Distinct())
            {
                _otaDataSync.SynOrderAsync(orderSend, parkId);
            }
            // _dataSyncOrderAppService.SendOrderFromParkApi(orderSend);
        }

        /// <summary>
        /// 取消确认时更新预订主订单和子订单
        /// </summary>
        /// <param name="toHeaderPre"></param>
        private void UpdateCancelTOHeaderPreAndTOBodyPre(TOHeaderPre toHeaderPre)
        {
            //更改预订单状态
            toHeaderPre.MainOrderState = MainOrderState.NotConfirm;
            toHeaderPre.ConfirmPersons = 0;
            toHeaderPre.ConfirmQty = 0;
            toHeaderPre.ConfirmFreePersons = 0;
            toHeaderPre.ConfirmFreeQty = 0;

            //预订子订单确认票数和人数归零
            toHeaderPre.TOBodyPres.ForEach(m => m.ConfirmPersons = 0);
            toHeaderPre.TOBodyPres.ForEach(m => m.ConfirmQty = 0);
            //预订子订单状态更改为待确认
            toHeaderPre.TOBodyPres.ForEach(m => m.OrderState = OrderState.WaitConfirm);
        }

        /// <summary>
        /// 确认订单时更新预订主订单和子订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<TOHeaderPre> UpdateTOHeaderPreAndTOBodyPre(ConfirmOrderInput input)
        {
            //更新主预订单和子预订单确认票数
            var toheaderPre = await _tOHeaderPreRepository.GetAllIncluding(m => m.TOBodyPres).FirstAsync(m => m.Id == input.Id);
            //toheaderPre.ConfirmPersons = input.Persons;
            //toheaderPre.ConfirmQty = input.Qty;
            toheaderPre.ConfirmPersons = input.TOBodyPres.Where(m => m.SalePrice != 0).Sum(m => m.Persons);
            toheaderPre.ConfirmFreePersons = input.TOBodyPres.Where(m => m.SalePrice == 0).Sum(m => m.Persons);
            toheaderPre.ConfirmQty = input.TOBodyPres.Where(m => m.SalePrice != 0).Sum(m => m.Qty);
            toheaderPre.ConfirmFreeQty = input.TOBodyPres.Where(m => m.SalePrice == 0).Sum(m => m.Qty);
            toheaderPre.MainOrderState = MainOrderState.Confirm;
            toheaderPre.Remark = input.ToHeadRemark;

            //更改预订子订单确认人数和数量
            foreach (var tobodyPre in toheaderPre.TOBodyPres)
            {
                //预订子订单状态确认后为待付款
                tobodyPre.OrderState = OrderState.WaitPay;
                foreach (var body in input.TOBodyPres)
                {
                    if (tobodyPre.AgencySaleTicketClassId.Equals(body.AgencySaleTicketClassId))
                    {
                        tobodyPre.ConfirmQty = body.Qty;
                        tobodyPre.ConfirmPersons = body.Persons;
                    }
                }
            }

            return toheaderPre;
        }

        /// <summary>
        /// 确认后生成新的子订单
        /// </summary>
        /// <param name="input"></param>
        private async void AddNewTOBody(ConfirmOrderInput input)
        {
            //确保子订单seq从1开始且连续的
            var j = 0;

            var toheaderId = input.Id;

            //生成确认后的子订单
            for (var i = 0; i < input.TOBodyPres.Count; i++)
            {
                //数量为0 的票不录入系统
                if (input.TOBodyPres[i].Qty.Equals(0))
                    continue;

                j += 1;
                input.TOBodyPres[i].Seq = j;
                input.TOBodyPres[i].TOHeaderId = toheaderId;
                input.TOBodyPres[i].Id = $"{toheaderId}{i + 1:D3}";
                //确认之后的订单状态为待付款
                input.TOBodyPres[i].OrderState = OrderState.WaitPay;
                await _tOBodyRepository.InsertAndGetIdAsync(input.TOBodyPres[i].MapTo<TOBody>());
            }
        }

        /// <summary>
        /// 确认后生成新的子订单
        /// </summary>
        /// <param name="toBodyPres"></param>
        private async void ConvertToTOBody(IEnumerable<TOBodyPre> toBodyPres)
        {
            //生成确认后的子订单
            var bodyPres = toBodyPres as TOBodyPre[] ?? toBodyPres.ToArray();

            var toHeaderPreId = bodyPres[0].TOHeaderPreId;
            for (var i = 0; i < bodyPres.Length; i++)
            {
                var centreOrderEditInput = new CentreOrderEditInput
                {
                    AgencySaleTicketClassId = bodyPres[i].AgencySaleTicketClassId,
                    Amount = bodyPres[i].Amount,
                    Id = $"{toHeaderPreId}{i + 1:D3}",
                    OrderState = OrderState.WaitPay,
                    ParkId = bodyPres[i].ParkId,
                    ParkSettlementPrice = bodyPres[i].ParkSettlementPrice,
                    Persons = bodyPres[i].Persons,
                    Price = bodyPres[i].Price,
                    Qty = bodyPres[i].Qty,
                    Remark = bodyPres[i].Remark,
                    SalePrice = bodyPres[i].SalePrice,
                    Seq = bodyPres[i].Seq,
                    SettlementPrice = bodyPres[i].SettlementPrice,
                    TOHeaderId = toHeaderPreId
                };
                await _tOBodyRepository.InsertAndGetIdAsync(centreOrderEditInput.MapTo<TOBody>());
            }
        }

        /// <summary>
        /// 确认后生成新的子订单
        /// </summary>
        /// <param name="toHeaderPre"></param>
        private void UpdateConfirmTOHeaderPreAndTOBodyPre(TOHeaderPre toHeaderPre)
        {
            toHeaderPre.ConfirmPersons = toHeaderPre.TOBodyPres.Where(m => m.SalePrice != 0).Sum(m => m.Persons);
            toHeaderPre.ConfirmFreePersons = toHeaderPre.TOBodyPres.Where(m => m.SalePrice == 0).Sum(m => m.Persons);
            toHeaderPre.ConfirmQty = toHeaderPre.TOBodyPres.Where(m => m.SalePrice != 0).Sum(m => m.Qty);
            toHeaderPre.ConfirmFreeQty = toHeaderPre.TOBodyPres.Where(m => m.SalePrice == 0).Sum(m => m.Qty);
            toHeaderPre.MainOrderState = MainOrderState.Confirm;

            Parallel.ForEach(toHeaderPre.TOBodyPres, (body, state) =>
            {
                body.OrderState = OrderState.WaitPay;
                body.ConfirmQty = body.Qty;
                body.ConfirmPersons = body.Persons;
            });
        }

        #endregion Private Methods
    }
}
