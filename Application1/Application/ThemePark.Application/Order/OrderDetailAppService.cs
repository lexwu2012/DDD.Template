using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using ThemePark.Application.Order.Dto;
using ThemePark.Application.Order.Interfaces;
using ThemePark.Application.Users;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.DataSync;
using ThemePark.Core.InPark;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Infrastructure.Web.Api;

namespace ThemePark.Application.Order
{
    /// <summary>
    /// 订单查询详情应用服务
    /// 支持查询全部线上订单，包括订单状态、入园明细（包括具体的入园时间点，XX分），入园时间点、取票/刷身份证/二维码、几号窗口/几号轧机。
    /// 出票类型：是电子票还是纸质票                     
    /// 一个订单多张票，需要每张票都显示入园信息
    /// </summary>
    public class OrderDetailAppService : ThemeParkAppServiceBase, IOrderDetailAppService
    {
        #region Fields
        private readonly IRepository<TOHeader, string> _tOHeaderRepository;
        private readonly IRepository<GroupTicket, string> _groupTicketRepository;
        private readonly IRepository<TOTicket, string> _tOTicketRepository;
        private readonly IRepository<TOVoucher, string> _tOVoucherRepository;
        private readonly IRepository<TicketRefund, string> _ticketRefundRepository;
        private readonly IRepository<TicketInPark, long> _ticketInParkRepository;
        private readonly IRepository<TORefund, long> _tORefundRepository;
        private readonly IRepository<SyncPark> _syncParkRepository;
        private readonly IRepository<TOHeaderPre, string> _tOHeaderPreRepository;
        private readonly IRepository<TOBodyPre, string> _tOBodyPreRepository;
        private readonly IRepository<Park> _parkRepository;
        //private IUserInfoProvider _userinfoProvider;
        //private IUserInfoService _userInfoService;
        //protected IUserInfoService UserInfoService => _userInfoService ?? (_userInfoService = _userinfoProvider.GetUserInfoService());
        #endregion

        #region cotr
        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="groupTicketRepository"></param>
        /// <param name="ticketInParkRepository"></param>
        /// <param name="tOTicketRepository"></param>
        /// <param name="tOVoucherRepository"></param>
        /// <param name="tOHeaderRepository"></param>
        /// <param name="ticketRefundRepository"></param>
        /// <param name="tORefundRepository"></param>
        /// <param name="tOHeaderPreRepository"></param>
        /// <param name="syncParkRepository"></param>
        /// <param name="parkRepository"></param>
        /// <param name="tOBodyPreRepository"></param>
        public OrderDetailAppService(IRepository<GroupTicket, string> groupTicketRepository, IRepository<TicketInPark, long> ticketInParkRepository,
            IRepository<TOTicket, string> tOTicketRepository, IRepository<TOVoucher, string> tOVoucherRepository,
            IRepository<TOHeader, string> tOHeaderRepository, IRepository<Core.ParkSale.TicketRefund, string> ticketRefundRepository,
            IRepository<TORefund, long> tORefundRepository, IRepository<SyncPark> syncParkRepository,
            IRepository<TOHeaderPre, string> tOHeaderPreRepository, IRepository<Park> parkRepository, IRepository<TOBodyPre, string> tOBodyPreRepository)
        {
            _groupTicketRepository = groupTicketRepository;
            _tOTicketRepository = tOTicketRepository;
            _tOVoucherRepository = tOVoucherRepository;
            _tOHeaderRepository = tOHeaderRepository;
            _ticketRefundRepository = ticketRefundRepository;
            _ticketInParkRepository = ticketInParkRepository;
            _tORefundRepository = tORefundRepository;
            _syncParkRepository = syncParkRepository;
            _tOHeaderPreRepository = tOHeaderPreRepository;
            _parkRepository = parkRepository;
            _tOBodyPreRepository = tOBodyPreRepository;
            //_userInfoService = userInfoService;
            //_userinfoProvider = userinfoProvider;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 中心通过API获取公园订单信息（由于票/入园等信息不是实时同步过来中心，所以需要通过公园API接口查询公园的票信息）
        /// </summary>
        /// <param name="toheaderId"></param>
        /// <returns></returns>
        public async Task<Result<OrderDetailDto>> GetOrderDetailFromCentreOrParkApiAsync(string toheaderId)
        {
            /*
             *  什么情况需要从公园api接口获取订单数据？
             *  由于当天出的票是第二天凌晨再同步到中心的，所以当中心已经有票了，就不需要去公园查了
             * 
             *  目前做法是所有信息都从公园里面取
             */

            if (string.IsNullOrWhiteSpace(toheaderId))
                return Result.FromError<OrderDetailDto>("订单号不能为空");

            TOHeader toheader;
            var orderDto = new OrderDetailDto();
            using (UnitOfWorkManager.Current.DisableFilter(DataFilters.ParkPermission, DataFilters.AgencyPermission))
            {
                toheader = await _tOHeaderRepository.AsNoTrackingAndInclude(m => m.TOBodies).FirstOrDefaultAsync(m => m.Id == toheaderId);
            }

            var toheaderPre = await _tOHeaderPreRepository.FirstOrDefaultAsync(toheaderId);

            //toheader(这里包括旅行社订单和OTA订单)为空的话，还有可能是旅行社的预订单没确认
            if (toheader == null)
            {
                if (toheaderPre == null)
                    return Result.FromCode<OrderDetailDto>(ResultCode.NoRecord);

                //返回预订单信息
                GetReserverOrderDetailFromCentre(toheaderPre, orderDto);
                return Result.FromData(orderDto);
            }

            //公园同步接口
            var syncParkId = toheader.TOBodies.First().ParkId;
            var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == syncParkId);
            var uri = new Uri(sync.SyncUrl);

            var requestUrl =
                $"/{ApiRouteConstant.ApiPrefix}{ApiRouteConstant.OrderBusinessRoute}/{ApiRouteConstant.FetchOrderDetailRoute}?toheaderId={toheaderId}";

            //通过API获取公园订单信息            
            var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), requestUrl, string.Empty);

            //请求本地api(测试用)
            //var baseAddress = "http://localhost:59029";
            //var response = await HttpHelper.PostAsync(baseAddress, requestUrl, string.Empty);

            var result = JsonConvert.DeserializeObject<Result<OrderDetailDto>>(response);

            //在中心这里查询旅行社订单预订数量，因为预订单没有同步到公园，所以只能在中心这里取
            if (result != null && result.Success && result.Data != null && result.Data.TOBodyDtosList.Count != 0 && result.Data.OrderType == OrderType.TravelOrder)
                GetReserverQtyAndStateFromCentre(result.Data, toheaderPre.MainOrderState);

            //OTA订单退款，torefund不会同步到公园，所以需要在中心查
            if (result != null && result.Success && result.Data != null && result.Data.OrderType == OrderType.OTAOrder &&
                toheader.TOBodies.Any(m => m.OrderState == OrderState.OrderRefunded))
            {
                var torefund = await _tORefundRepository.GetAll().FirstAsync(m => m.TOHeaderId == toheaderId);
                Parallel.ForEach(result.Data.TOBodyDtosList, (body, state, i) =>
                {
                    body.TicketDetailDtos.ForEach(o => o.TicketRefundInfo = new RefundInfo
                    {
                        RefundTime = torefund.CreationTime
                    });
                });
            }

            return result;
        }


        /// <summary>
        /// 获取订单当前状态
        /// </summary>
        /// <param name="toheaderId"></param>
        /// <returns>所有子订单状态信息</returns>
        public async Task<Result<OrderDetailDto>> GetOrderDetailAsync(string toheaderId)
        {
            /*
             * 1. 验证订单号（跟预订单有关？）
             * 2. 旅行社/OTA订单可以作为树形展示，因为toticket/groupticket是可以追溯到哪个子订单生成的              
             * 3. 查看有没有出票，如果有，则跟踪票状态，没有则查看订单状态
             * 4. 订单类型为旅行社的话，追踪groupticket，为OTA订单的话，追踪toticket
             */

            if (string.IsNullOrWhiteSpace(toheaderId))
                return Result.FromError<OrderDetailDto>("订单号不能为空");

            var orderDto = new OrderDetailDto();
            TOHeader toheader;
            using (UnitOfWorkManager.Current.DisableFilter(DataFilters.ParkPermission, DataFilters.AgencyPermission))
            {
                toheader = await _tOHeaderRepository.AsNoTrackingAndInclude(m => m.TOBodies).FirstOrDefaultAsync(m => m.Id == toheaderId);
            }
            //toheader = await _tOHeaderRepository.AsNoTrackingAndInclude(m => m.TOBodies).FirstOrDefaultAsync(m => m.Id == toheaderId);
            //toheader为空的话，还有可能是预订单没确认
            if (toheader == null)
                return Result.FromError<OrderDetailDto>("公园没有此订单信息");

            orderDto.CreationTime = toheader.CreationTime;
            orderDto.TOHeaderId = toheaderId;
            orderDto.Remark = toheader.Remark;
            orderDto.ParkName = await GetParkName(toheader.TOBodies.First().ParkId);

            //OTA订单和旅行社订单
            switch (toheader.OrderType)
            {
                case OrderType.TravelOrder:
                    TraceTravelOrder(toheader, orderDto);
                    orderDto.ConfirmAmount = toheader.Amount;
                    orderDto.ConfirmPersons = toheader.Persons;
                    orderDto.ConfirmQty = toheader.Qty;
                    orderDto.OrderType = OrderType.TravelOrder;
                    break;
                case OrderType.OTAOrder:
                    orderDto.OrderType = OrderType.OTAOrder;
                    TraceOtaOrder(toheader, orderDto);
                    break;
            }

            return Result.FromData(orderDto);
        }


        public async Task<Result> GetTOBodyDetailById(string tobodyId)
        {

            if (string.IsNullOrWhiteSpace(tobodyId))
                return Result.FromError<OrderDetailDto>("订单号不能为空");

            using (UnitOfWorkManager.Current.DisableFilter(DataFilters.ParkPermission, DataFilters.AgencyPermission))
            {

            }

            return Result.Ok();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// 追踪生成团体票的旅行社订单
        /// </summary>
        /// <param name="toheader"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        private void TraceTravelOrder(TOHeader toheader, OrderDetailDto dto)
        {
            /*
             *   旅行社订单中在窗口取票，在生成团体票时把所有的子订单状态都置为相同
             */
            foreach (var body in toheader.TOBodies)
            {
                var tobodyDetailDto = body.MapTo<TOBodyDetailDto>();
                GetTravleTOBodyAndGroupTicketDetail(dto, tobodyDetailDto);
                //订单待确认，订单已确认，订单已出票，订单已退款
            }
        }

        /// <summary>
        /// 跟踪查询OTA订单
        /// </summary>
        /// <param name="toheader"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        private void TraceOtaOrder(TOHeader toheader, OrderDetailDto dto)
        {
            //toheader -> tobody -> tovoucher -> toticket
            foreach (var body in toheader.TOBodies)
            {
                //订单已冻结，订单待确认(OTA订单没有这个状态)，订单待付款，订单待消费，订单已消费（已取票），订单已退款
                switch (body.OrderState)
                {
                    //订单已冻结（只有主订单和子订单的信息可查询）
                    case OrderState.Freezon:
                    //订单待付款(此时没有生成TOTicket电子票)
                    case OrderState.WaitPay:
                    //订单待消费(付完款未取票)
                    case OrderState.WaitCost:
                    //OTA订单已退款(TORefund不会同步到公园来，所以只有直接取票信息)
                    case OrderState.OrderRefunded:
                    //订单已消费（就是已经出票）
                    case OrderState.TradeSuccess:
                        GetOtaTOBodyAndTOTicketDetail(dto, body.MapTo<TOBodyDetailDto>());
                        break;
                }
            }

        }

        /// <summary>
        /// 获取ota子订单信息和票信息（如果有的话）
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private void GetOtaTOBodyAndTOTicketDetail(OrderDetailDto dto, TOBodyDetailDto body)
        {
            List<string> tovoucherIds;
            using (UnitOfWorkManager.Current.DisableFilter(DataFilters.ParkPermission))
            {
                tovoucherIds = _tOVoucherRepository.AsNoTracking().Where(m => body.Id == m.TOBodyId).Select(m => m.Id).ToList();
            }

            var totickets = _tOTicketRepository.AsNoTracking().Where(m => tovoucherIds.Contains(m.TOVoucherId)).ToList();

            //子订单下生成的票（子订单为状态为未付款的totickets为空，所以直接返回子订单信息，没有票）
            foreach (var toticket in totickets)
            {
                //预售，有效，已入园，已作废，已过期，已退票
                switch (toticket.TicketSaleStatus)
                {
                    //查询TicketInPark
                    case TicketSaleStatus.InPark:
                        GetTOTicketInParkInfo(body, toticket);
                        break;
                    //有效票会有已经入园的情况，只有全部入园了票状态才置为已入园
                    case TicketSaleStatus.Valid:
                        GetValidTOTicketInfo(body, toticket);
                        break;
                    case TicketSaleStatus.Expire:
                    //(网络票退票，由于退票信息torefund没有同步到公园只留在中心，所以只能取订单信息)
                    case TicketSaleStatus.Refund:
                    case TicketSaleStatus.Freezon:
                        GetTOTicketInfo(body, toticket);
                        break;
                    case TicketSaleStatus.Invalid:
                        if (!body.OrderState.Equals(OrderState.TradeSuccess))
                        {
                            GetTOTicketInfo(body, toticket);
                        }
                        break;
                }
            }

            //子订单
            dto.TOBodyDtosList.Add(body);
        }

        /// <summary>
        /// 获取旅行社子订单信息和票信息（如果有的话）
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private void GetTravleTOBodyAndGroupTicketDetail(OrderDetailDto dto, TOBodyDetailDto body)
        {
            List<GroupTicket> groupTickets;
            using (UnitOfWorkManager.Current.DisableFilter(DataFilters.ParkPermission))
            {
                groupTickets = _groupTicketRepository.GetAll().Where(m => m.TOBodyId == body.Id).ToList();
                //Logger.Log(LogSeverity.Info, "主订单：" + body.TOHeaderId + ",子订单：" + body.Id + ", groupTickets数量：" + groupTickets.Count);
            }

            //实际出票数量
            body.WarrantQty = groupTickets.Sum(m => m.Qty);
            body.WarrantAmount = groupTickets.Sum(m => m.Amount);

            //子订单下生成的票（子订单为状态为未付款的totickets为空，所以直接返回子订单信息，没有票）
            foreach (var groupTicket in groupTickets)
            {
                //预售，有效，已入园，已作废，已过期，已退票
                switch (groupTicket.TicketSaleStatus)
                {
                    //查询TicketInPark
                    case TicketSaleStatus.InPark:
                        GetGroupTicketInParkInfo(body, groupTicket);
                        break;
                    //有效票会有已经入园的情况，只有全部入园了票状态才置为已入园
                    case TicketSaleStatus.Valid:
                        GetValidTravelTicketInfo(body, groupTicket);
                        break;
                    case TicketSaleStatus.PreSale:
                    case TicketSaleStatus.Expire:
                        GetGroupTicketInfo(body, groupTicket);
                        break;
                    //票已退款
                    case TicketSaleStatus.Refund:
                        GetGroupTicketRefundInfo(body, groupTicket);
                        break;
                }
            }
            //子订单
            dto.TOBodyDtosList.Add(body);
        }

        /// <summary>
        /// 获取网络票的信息
        /// </summary>
        /// <param name="bodyDto"></param>
        /// <param name="toticket"></param>
        private async void GetTOTicketInfo(TOBodyDetailDto bodyDto, TOTicket toticket)
        {
            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            var ticketDetail = new TicketDetailDto
            {
                Barcode = toticket.Id,
                TicketSaleStatusName = toticket.TicketSaleStatus.DisplayName(),
                //出票类型
                TicketFormEnumName = toticket.TicketFormEnum.DisplayName(),
                Amount = toticket.Amount,
                Qty = toticket.Qty,
                ValidStartDate = toticket.ValidStartDate,
                ValidDays = toticket.ValidDays,
                CreationTime = toticket.CreationTime,
                CreatorName = toticket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(toticket.CreatorUserId.Value)
            };
            bodyDto.TicketDetailDtos.Add(ticketDetail);
        }

        /// <summary>
        /// 获取网络票的信息
        /// </summary>
        /// <param name="bodyDto"></param>
        /// <param name="groupTicket"></param>
        private async void GetGroupTicketInfo(TOBodyDetailDto bodyDto, GroupTicket groupTicket)
        {
            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            var ticketDetail = new TicketDetailDto
            {
                Barcode = groupTicket.Id,
                TicketSaleStatusName = groupTicket.TicketSaleStatus.DisplayName(),
                Amount = groupTicket.Amount,
                Qty = groupTicket.Qty,
                ValidStartDate = groupTicket.ValidStartDate,
                ValidDays = groupTicket.ValidDays,
                CreationTime = groupTicket.CreationTime,
                CreatorName = groupTicket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(groupTicket.CreatorUserId.Value)
            };
            bodyDto.TicketDetailDtos.Add(ticketDetail);
        }


        /// <summary>
        /// 网络票入园信息
        /// </summary>
        /// <param name="bodyDto"></param>
        /// <param name="toticket"></param>
        private async void GetTOTicketInParkInfo(TOBodyDetailDto bodyDto, TOTicket toticket)
        {
            //从票中获取入园时间点、取票/刷身份证/二维码、几号窗口/几号轧机
            //入园记录有terminalId，入园闸口
            //一张票可多次入园，去所有的入园信息
            var ticketInparks = await _ticketInParkRepository.GetAll().Where(m => m.Barcode == toticket.Id).OrderBy(m => m.CreationTime).ToListAsync();

            List<TicketInParkInfo> ticketInParkInfo = new List<TicketInParkInfo>();
            foreach (var ticketInpark in ticketInparks)
            {
                ticketInParkInfo.Add(new TicketInParkInfo
                {
                    InParkChannel = ticketInpark.TerminalId.ToString(),
                    InParkTime = ticketInpark.CreationTime,
                    Qty = ticketInpark.Qty,
                    ParkName = await GetParkName(ticketInpark.ParkId)
                });
            }
            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            bodyDto.TicketDetailDtos.Add(new TicketDetailDto
            {
                Barcode = toticket.Id,
                TicketInParkInfos = ticketInParkInfo,
                TicketFormEnumName = toticket.TicketFormEnum.DisplayName(),
                TicketSaleStatusName = toticket.TicketSaleStatus.DisplayName(),
                CreationTime = toticket.CreationTime,
                Qty = toticket.Qty,
                CreatorName = toticket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(toticket.CreatorUserId.Value),
                ValidStartDate = toticket.ValidStartDate,
                ValidDays = toticket.ValidDays
            });
        }

        /// <summary>
        /// 团体票入园信息
        /// </summary>
        /// <param name="bodyDto"></param>
        /// <param name="groupTicket"></param>
        private async void GetGroupTicketInParkInfo(TOBodyDetailDto bodyDto, GroupTicket groupTicket)
        {
            //从票中获取入园时间点、取票/刷身份证/二维码、几号窗口/几号轧机
            //入园记录有terminalId，入园闸口
            //一张票可多次入园，取第一次入园信息？
            var ticketInparks = await _ticketInParkRepository.GetAll().Where(m => m.Barcode == groupTicket.Id).OrderBy(m => m.CreationTime).ToListAsync();

            List<TicketInParkInfo> ticketInParkInfos = new List<TicketInParkInfo>();
            foreach (var ticketInpark in ticketInparks)
            {
                ticketInParkInfos.Add(new TicketInParkInfo
                {
                    InParkChannel = ticketInpark.TerminalId.ToString(),
                    InParkTime = ticketInpark.CreationTime,
                    Qty = ticketInpark.Qty,
                    ParkName = await GetParkName(ticketInpark.ParkId)
                });
            }
            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            bodyDto.TicketDetailDtos.Add(new TicketDetailDto
            {
                Barcode = groupTicket.Id,
                TicketInParkInfos = ticketInParkInfos,
                TicketSaleStatusName = groupTicket.TicketSaleStatus.DisplayName(),
                CreationTime = groupTicket.CreationTime,
                Qty = groupTicket.Qty,
                CreatorName = groupTicket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(groupTicket.CreatorUserId.Value),
                ValidStartDate = groupTicket.ValidStartDate,
                ValidDays = groupTicket.ValidDays
            });
        }

        /// <summary>
        /// 获取票的退款信息
        /// </summary>
        /// <param name="bodyDetailDto"></param>
        /// <param name="groupTicket"></param>
        private async void GetGroupTicketRefundInfo(TOBodyDetailDto bodyDetailDto, GroupTicket groupTicket)
        {
            var refund = await _ticketRefundRepository.FirstOrDefaultAsync(m => m.Id == groupTicket.Id);
            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            var ticketDetail = new TicketDetailDto
            {
                Barcode = groupTicket.Id,
                TicketSaleStatusName = TicketSaleStatus.Refund.DisplayName(),
                //退款信息
                TicketRefundInfo = new RefundInfo
                {
                    RefundTime = refund.CreationTime,
                    Amount = refund.Amount,
                    Reason = refund.Reason,
                    CreatorUserName = refund.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(refund.CreatorUserId.Value)
                },
                Amount = groupTicket.Amount,
                Qty = groupTicket.Qty,
                ValidStartDate = groupTicket.ValidStartDate,
                ValidDays = groupTicket.ValidDays,
                CreationTime = groupTicket.CreationTime,
                CreatorName = groupTicket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(groupTicket.CreatorUserId.Value)
            };
            bodyDetailDto.TicketDetailDtos.Add(ticketDetail);
            bodyDetailDto.RefundQty = bodyDetailDto.RefundQty + groupTicket.Qty;
            bodyDetailDto.RefundAmount = bodyDetailDto.RefundAmount + refund.Amount;
        }

        /// <summary>
        /// 获取票的退款信息
        /// </summary>
        /// <param name="bodyDetailDto"></param>
        /// <param name="toTicket"></param>
        private async void GetTOTicketRefundInfo(TOBodyDetailDto bodyDetailDto, TOTicket toTicket)
        {
            //var refund = await _ticketRefundRepository.FirstOrDefaultAsync(m => m.Id == toTicket.Id);
            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            var ticketDetail = new TicketDetailDto
            {
                Barcode = toTicket.Id,
                TicketSaleStatusName = TicketSaleStatus.Refund.DisplayName(),
                TicketFormEnumName = toTicket.TicketFormEnum.DisplayName(),
                ////退款信息
                //TicketRefundInfo = new RefundInfo
                //{
                //    RefundTime = refund.CreationTime,
                //    Amount = refund.Amount,
                //    Reason = refund.Reason,
                //    CreatorUserName = refund.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(refund.CreatorUserId.Value)
                //},
                Amount = toTicket.Amount,
                Qty = toTicket.Qty,
                ValidStartDate = toTicket.ValidStartDate,
                ValidDays = toTicket.ValidDays,
                CreationTime = toTicket.CreationTime,
                CreatorName = toTicket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(toTicket.CreatorUserId.Value)
            };
            bodyDetailDto.TicketDetailDtos.Add(ticketDetail);
        }

        /// <summary>
        /// OTA获取订单退款信息，OTA一旦取了票就不能退了，没取票之前是可以退单的，退单后电子票（TOTicket）更改状态为已退票
        /// （OTA一旦下单了就会生成TOTicket（电子票））
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="body"></param>
        private async void GetOrderRefund(OrderDetailDto dto, TOBodyDetailDto body)
        {
            var refund = await _tORefundRepository.AsNoTracking().Where(m => m.TOHeaderId == body.TOHeaderId).FirstOrDefaultAsync();

            body.OrderRefundInfo = new RefundInfo
            {
                Amount = refund.Amount,
                RefundTime = refund.CreationTime
            };
            dto.TOBodyDtosList.Add(body);
        }

        /// <summary>
        /// 获取预订单信息
        /// </summary>
        /// <param name="toHeaderPre"></param>
        /// <param name="orderDetailDto"></param>
        private void GetReserverOrderDetailFromCentre(TOHeaderPre toHeaderPre, OrderDetailDto orderDetailDto)
        {
            foreach (var toBodyPres in toHeaderPre.TOBodyPres)
            {
                var tobodyPre = toBodyPres.MapTo<TOBodyPreDetailDto>();
                orderDetailDto.TOBodyPreDtosList.Add(tobodyPre);
            }

            orderDetailDto.ParkName = orderDetailDto.TOBodyPreDtosList.First().ParkName;
            orderDetailDto.CreationTime = toHeaderPre.CreationTime;
            orderDetailDto.TOHeaderId = toHeaderPre.Id;
            orderDetailDto.Remark = toHeaderPre.Remark;
            orderDetailDto.MainOrderState = toHeaderPre.MainOrderState;
        }

        /// <summary>
        /// 获取公园名称
        /// </summary>
        /// <param name="parkId"></param>
        /// <returns></returns>
        private async Task<string> GetParkName(int parkId)
        {
            var park = await _parkRepository.FirstOrDefaultAsync(parkId);

            return park == null ? string.Empty : park.ParkName;
        }

        /// <summary>
        /// 从中心获取预订子订单的预订票数量
        /// </summary>
        /// <param name="detailDto"></param>
        /// <param name="mainOrderState"></param>
        private async void GetReserverQtyAndStateFromCentre(OrderDetailDto detailDto, MainOrderState mainOrderState)
        {
            foreach (var body in detailDto.TOBodyDtosList)
            {
                var tOBodyPre = await _tOBodyPreRepository.AsNoTracking()
                    .Where(m => m.AgencySaleTicketClassId == body.AgencySaleTicketClassId && m.TOHeaderPreId == body.TOHeaderId).FirstOrDefaultAsync();

                //确认订单的预订数量
                body.ReserverQty = tOBodyPre == null ? 0 : tOBodyPre.Qty;
            }
            detailDto.MainOrderState = mainOrderState;
        }


        /// <summary>
        /// 从中心获取ota订单退款信息
        /// </summary>
        /// <param name="detailDto"></param>
        /// <param name="mainOrderState"></param>
        private async void GetOtaRefundInfoFromCentre(OrderDetailDto detailDto)
        {
            foreach (var body in detailDto.TOBodyDtosList)
            {
                var tOBodyPre = await _tOBodyPreRepository.AsNoTracking()
                    .Where(m => m.AgencySaleTicketClassId == body.AgencySaleTicketClassId && m.TOHeaderPreId == body.TOHeaderId).FirstOrDefaultAsync();

                //确认订单的预订数量
                body.ReserverQty = tOBodyPre == null ? 0 : tOBodyPre.Qty;
            }
        }

        /// <summary>
        /// 判断旅行社有效票是否已经有入园记录
        /// </summary>
        /// <param name="bodyDto"></param>
        /// <param name="groupTicket"></param>
        private async void GetValidTravelTicketInfo(TOBodyDetailDto bodyDto, GroupTicket groupTicket)
        {
            var ticketInparks =
                await _ticketInParkRepository.GetAll()
                    .Where(m => m.Barcode == groupTicket.Id)
                    .OrderBy(m => m.CreationTime)
                    .ToListAsync();

            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            if (ticketInparks.Count != 0)
            {
                var ticketInParkInfos = new List<TicketInParkInfo>();
                foreach (var ticketInpark in ticketInparks)
                {
                    ticketInParkInfos.Add(new TicketInParkInfo
                    {
                        InParkChannel = ticketInpark.TerminalId.ToString(),
                        InParkTime = ticketInpark.CreationTime,
                        Qty = ticketInpark.Qty,
                        ParkName = await GetParkName(ticketInpark.ParkId)
                    });
                }
                bodyDto.TicketDetailDtos.Add(new TicketDetailDto
                {
                    Barcode = groupTicket.Id,
                    TicketInParkInfos = ticketInParkInfos,
                    TicketSaleStatusName = groupTicket.TicketSaleStatus.DisplayName(),
                    CreationTime = groupTicket.CreationTime,
                    Qty = groupTicket.Qty,
                    CreatorName =
                        groupTicket.CreatorUserId == null
                            ? string.Empty
                            : await userInfoService.Object.GetUserNameByIdAsync(groupTicket.CreatorUserId.Value),
                    ValidStartDate = groupTicket.ValidStartDate,
                    ValidDays = groupTicket.ValidDays
                });
            }
            else
            {
                bodyDto.TicketDetailDtos.Add(new TicketDetailDto
                {
                    Barcode = groupTicket.Id,
                    TicketSaleStatusName = groupTicket.TicketSaleStatus.DisplayName(),
                    Amount = groupTicket.Amount,
                    Qty = groupTicket.Qty,
                    ValidStartDate = groupTicket.ValidStartDate,
                    ValidDays = groupTicket.ValidDays,
                    CreationTime = groupTicket.CreationTime,
                    CreatorName =
                        groupTicket.CreatorUserId == null
                            ? string.Empty
                            : await userInfoService.Object.GetUserNameByIdAsync(groupTicket.CreatorUserId.Value)
                });
            }
        }

        /// <summary>
        /// 判断网络有效票是否已经有入园记录
        /// </summary>
        /// <param name="bodyDto"></param>
        /// <param name="toticket"></param>
        private async void GetValidTOTicketInfo(TOBodyDetailDto bodyDto, TOTicket toticket)
        {
            //从票中获取入园时间点、取票/刷身份证/二维码、几号窗口/几号轧机
            //入园记录有terminalId，入园闸口
            //一张票可多次入园，去所有的入园信息
            var ticketInparks = await _ticketInParkRepository.GetAll().Where(m => m.Barcode == toticket.Id).OrderBy(m => m.CreationTime).ToListAsync();

            var userInfoService = IocManager.Instance.ResolveAsDisposable<IUserAppService>();
            if (ticketInparks.Count != 0)
            {
                var ticketInParkInfo = new List<TicketInParkInfo>();
                foreach (var ticketInpark in ticketInparks)
                {
                    ticketInParkInfo.Add(new TicketInParkInfo
                    {
                        InParkChannel = ticketInpark.TerminalId.ToString(),
                        InParkTime = ticketInpark.CreationTime,
                        Qty = ticketInpark.Qty,
                        ParkName = await GetParkName(ticketInpark.ParkId)
                    });
                }
                bodyDto.TicketDetailDtos.Add(new TicketDetailDto
                {
                    Barcode = toticket.Id,
                    TicketInParkInfos = ticketInParkInfo,
                    TicketFormEnumName = toticket.TicketFormEnum.DisplayName(),
                    TicketSaleStatusName = toticket.TicketSaleStatus.DisplayName(),
                    CreationTime = toticket.CreationTime,
                    Qty = toticket.Qty,
                    CreatorName = toticket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(toticket.CreatorUserId.Value),
                    ValidStartDate = toticket.ValidStartDate,
                    ValidDays = toticket.ValidDays
                });
            }
            else
            {
                var ticketDetail = new TicketDetailDto
                {
                    Barcode = toticket.Id,
                    TicketSaleStatusName = toticket.TicketSaleStatus.DisplayName(),
                    //出票类型
                    TicketFormEnumName = toticket.TicketFormEnum.DisplayName(),
                    Amount = toticket.Amount,
                    Qty = toticket.Qty,
                    ValidStartDate = toticket.ValidStartDate,
                    ValidDays = toticket.ValidDays,
                    CreationTime = toticket.CreationTime,
                    CreatorName = toticket.CreatorUserId == null ? string.Empty : await userInfoService.Object.GetUserNameByIdAsync(toticket.CreatorUserId.Value)
                };
                bodyDto.TicketDetailDtos.Add(ticketDetail);
            }
        }

        #endregion
    }
}
