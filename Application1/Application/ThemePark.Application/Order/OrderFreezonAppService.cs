using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using ThemePark.Application.DataSync;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.Order.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.DataSync;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Common;
using ThemePark.Infrastructure.Web.Api;

namespace ThemePark.Application.Order
{
    /// <summary>
    /// 订单冻结（解冻）应用服务
    /// </summary>
    public class OrderFreezonAppService : ThemeParkAppServiceBase, IOrderFreezonAppService
    {
        #region Fields
        private readonly IRepository<TOHeader, string> _tOHeaderRepository;
        private readonly IRepository<TOTicket, string> _tOTicketRepository;
        private readonly IRepository<TOVoucher, string> _tOVoucherRepository;
        private readonly IRepository<SyncPark> _syncParkRepository;
        #endregion

        #region cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tOHeaderRepository"></param>
        /// <param name="syncParkRepository"></param>
        /// <param name="tOTicketRepository"></param>
        /// <param name="tOVoucherRepository"></param>
        public OrderFreezonAppService(IRepository<TOHeader, string> tOHeaderRepository, IRepository<SyncPark> syncParkRepository, IRepository<TOTicket, string> tOTicketRepository,
            IRepository<TOVoucher, string> tOVoucherRepository)
        {
            _tOHeaderRepository = tOHeaderRepository;
            _tOTicketRepository = tOTicketRepository;
            _tOVoucherRepository = tOVoucherRepository;
            _syncParkRepository = syncParkRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 冻结订单
        /// </summary>
        /// <param name="tohearderId"></param>
        /// <returns></returns>
        public async Task<Result> FreezonOrderAsync(string tohearderId)
        {
            /*
             * 1. 只针对OTA，旅行社不用冻结
             * 2. 冻结子订单
             * 3. 冻结后对其他业务影响？
             * 4. 冻结电子票
             * 
             *  冻结后需把订单状态同步到公园
             */

            var toheader = await _tOHeaderRepository.FirstOrDefaultAsync(tohearderId);
            if (toheader == null)
                return Result.FromCode(ResultCode.NoRecord);

            //任一单不是待消费状态，不允许整单冻结
            if (toheader.TOBodies.Any(m => m.OrderState != OrderState.WaitCost))
            {
                return Result.FromCode(ResultCode.InvalidData, "不允许对该订单进行冻结");
            }

            //if (toheader.TOBodies.Any(m => m.OrderState == OrderState.Freezon))
            //    return Result.FromCode(ResultCode.InvalidData, "不允许对已冻结订单进行冻结");

            ////防止公园已经取票而中心页面订单停留在订单待消费状态进而可以冻结这种情况
            //if (toheader.TOBodies.Any(m => m.OrderState == OrderState.TradeSuccess))
            //    return Result.FromCode(ResultCode.InvalidData, "不允许对已消费订单进行冻结");

            //if (toheader.TOBodies.Any(m => m.OrderState == OrderState.OrderRefunded))
            //    return Result.FromCode(ResultCode.InvalidData, "不允许对已消费订单进行冻结");

            //冻结每个子订单
            EnumerableExtensions.ForEach(toheader.TOBodies, m => m.OrderState = OrderState.Freezon);

            //冻结电子票
            var bodyIds = toheader.TOBodies.Select(m => m.Id);

            var tovoucherIds = _tOVoucherRepository.AsNoTracking().Where(m => bodyIds.Contains(m.TOBodyId)).Select(m => m.Id).ToList();

            var totickets = _tOTicketRepository.AsNoTracking().Where(m => tovoucherIds.Contains(m.TOVoucherId)).ToList();

            totickets.ForEach(m => m.TicketSaleStatus = TicketSaleStatus.Freezon);

            //同步到公园
            var defaultParkId = toheader.TOBodies.Select(o => o.ParkId).First();

            var dto = new FreezeOrderDto
            {
                ParkId = defaultParkId,
                TOHeaderId = toheader.Id,
                DataSyncType = DataSyncType.OrderFreeze
            };            

            //公园同步接口
            var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == defaultParkId);
            var uri = new Uri(sync.SyncUrl);

            var requestUrl =
                $"/{ApiRouteConstant.ApiPrefix}{ApiRouteConstant.OrderBusinessRoute}/{ApiRouteConstant.FreezeOrderRoute}";

            var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), requestUrl, JsonConvert.SerializeObject(dto));

            //本地请求api
            //var baseAddress = "http://localhost:59029";
            //var response = await HttpHelper.PostAsync(baseAddress, requestUrl, JsonConvert.SerializeObject(dto));

            var result = JsonConvert.DeserializeObject<Result>(response);
            
            return result;
        }

        /// <summary>
        /// 解冻订单
        /// </summary>
        /// <param name="tohearderId"></param>
        /// <returns></returns>
        public async Task<Result> UnFreezonOrderAsync(string tohearderId)
        {
            /*
             * 1. 解冻子订单
             * 2. 解冻后对其他业务影响？
             * 
             *  解冻后需把订单状态同步到公园
             */

            var toheader = await _tOHeaderRepository.FirstOrDefaultAsync(tohearderId);
            if (toheader == null)
                return Result.FromCode(ResultCode.NoRecord);

            if (toheader.TOBodies.Any(m => m.OrderState != OrderState.Freezon))
                return Result.FromCode(ResultCode.InvalidData, "不允许对非冻结订单进行解冻");


            //解冻每个子订单
            EnumerableExtensions.ForEach(toheader.TOBodies, m => m.OrderState = OrderState.WaitCost);

            //冻结电子票
            var bodyIds = toheader.TOBodies.Select(m => m.Id);

            var tovoucherIds = _tOVoucherRepository.AsNoTracking().Where(m => bodyIds.Contains(m.TOBodyId)).Select(m => m.Id).ToList();

            var totickets = _tOTicketRepository.AsNoTracking().Where(m => tovoucherIds.Contains(m.TOVoucherId)).ToList();

            totickets.ForEach(m => m.TicketSaleStatus = TicketSaleStatus.Valid);

            //同步到公园
            var defaultParkId = toheader.TOBodies.Select(o => o.ParkId).First();

            var dto = new FreezeOrderDto
            {
                ParkId = defaultParkId,
                TOHeaderId = toheader.Id,
                DataSyncType = DataSyncType.OrderUnFreeze
            };

            //公园同步接口
            var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == defaultParkId);
            var uri = new Uri(sync.SyncUrl);

            var requestUrl =
               $"/{ApiRouteConstant.ApiPrefix}{ApiRouteConstant.OrderBusinessRoute}/{ApiRouteConstant.FreezeOrderRoute}";

            var response = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), requestUrl, JsonConvert.SerializeObject(dto));

            //本地请求api
            //var baseAddress = "http://localhost:59029";
            //var response = await HttpHelper.PostAsync(baseAddress, requestUrl, JsonConvert.SerializeObject(dto));

            var result = JsonConvert.DeserializeObject<Result>(response);
            
            return result;
        }

        #endregion
    }
}
