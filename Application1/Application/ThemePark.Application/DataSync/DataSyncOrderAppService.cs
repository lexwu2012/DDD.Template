using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ThemePark.Application.DataSync.Dto;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Newtonsoft.Json;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Infrastructure.Application;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.DataSync;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Infrastructure.Web.Api;
using EnumerableExtensions = ThemePark.Common.EnumerableExtensions;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 订单同步服务
    /// </summary>
    public class DataSyncOrderAppService : ThemeParkAppServiceBase, IDataSyncOrderAppService
    {
        #region Fields
        private readonly IRepository<TOHeader, string> _toHeaderRepository;
        private readonly IRepository<TOTicket, string> _toTicketRepository;
        private readonly IRepository<TOBody, string> _toBodyRepository;
        private readonly IRepository<TOBodyPre, string> _toBodyPreRepository;
        private readonly IRepository<TOVoucher, string> _toVoucherRepository;
        private readonly IRepository<GroupInfo, long> _groupInfoRepository;
        private readonly IRepository<NonGroupTicket, string> _nonGroupTicketRepository;
        private readonly IRepository<GroupTicket, string> _groupTicketRepository;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IAgencyAccountAppService _agencyAccountAppService;
        private readonly IRepository<ParkAgency> _parkAgencyRepository;
        private readonly IRepository<TOHeaderPre, string> _toHeaderPreRepository;
        #endregion Fields

        #region Cotr

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="toHeaderRepository"></param>
        /// <param name="toTicketRepository"></param>
        /// <param name="toBodyRepository"></param>
        /// <param name="toVoucherRepository"></param>
        public DataSyncOrderAppService(IRepository<TOHeader, string> toHeaderRepository, IRepository<TOTicket, string> toTicketRepository,
            IRepository<TOBody, string> toBodyRepository, IRepository<TOVoucher, string> toVoucherRepository, IRepository<GroupInfo, long> groupInfoRepository,
            IRepository<NonGroupTicket, string> nonGroupTicketRepository, IRepository<TOBodyPre, string> toBodyPreRepository,
            IDataSyncManager dataSyncManager, IAgencyAccountAppService agencyAccountAppService, IRepository<ParkAgency> parkAgencyRepository, IRepository<TOHeaderPre, string> toHeaderPreRepository, IRepository<GroupTicket, string> groupTicketRepository)
        {
            _toHeaderRepository = toHeaderRepository;
            _toTicketRepository = toTicketRepository;
            _toBodyRepository = toBodyRepository;
            _toVoucherRepository = toVoucherRepository;
            _groupInfoRepository = groupInfoRepository;
            _nonGroupTicketRepository = nonGroupTicketRepository;
            _toBodyPreRepository = toBodyPreRepository;
            _dataSyncManager = dataSyncManager;
            _agencyAccountAppService = agencyAccountAppService;
            _parkAgencyRepository = parkAgencyRepository;
            _toHeaderPreRepository = toHeaderPreRepository;
            _groupTicketRepository = groupTicketRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 核销订单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> OrderConsume(OrderConsumeDto dto)
        {
            using (UnitOfWorkManager.Current.DisableFilter(DataFilters.ParkPermission))
            {
                //OTA订单
                if (!string.IsNullOrWhiteSpace(dto.SubOrderid))
                {
                    var subOrderEntity = await _toBodyRepository.FirstOrDefaultAsync(m => m.Id == dto.SubOrderid);
                    if (subOrderEntity == null)
                        return Result.FromError("无相应的订单信息");
                    else if (subOrderEntity.OrderState == OrderState.TradeSuccess)//已核销订单不再核销
                        return Result.Ok();
                    else
                    {
                        //一个主订单可能包含多个子订单，只要任何一个子订单发生消费就会从预付填款余额中扣减全部的主订单金额
                        //所以只要一个主订单中任何子订单状态为已消费，就不会再次触发扣款操作
                        subOrderEntity.OrderState = OrderState.TradeSuccess; //设置当前子订单为已消费状态
                        var mainOrderEntity = await _toHeaderRepository
                            .GetAllIncluding(m => m.TOBodies)
                            .FirstOrDefaultAsync((m => m.Id == subOrderEntity.TOHeaderId));
                        if (mainOrderEntity.TOBodies.Any(k => k.OrderState == OrderState.TradeSuccess && k.Id != subOrderEntity.Id))
                            return Result.Ok();
                        else
                        {
                            var agencyId = mainOrderEntity.AgencyId;
                            var agencyType = await _parkAgencyRepository.AsNoTracking()
                                .Where(m => m.ParkId == subOrderEntity.ParkId && m.AgencyId == agencyId)
                                .Select(m => m.AgencyType)
                                .FirstAsync();

                            if (agencyType.DefaultAgencyType == DefaultAgencyType.Ota)
                            {
                                var preAccountDeductMoneyDto = new PreAccountDeductMoneyDto
                                {
                                    OrderId = mainOrderEntity.Id,
                                    TotalMoney = mainOrderEntity.Amount,
                                    AgencyId = agencyId
                                };

                                return await _agencyAccountAppService.DeductMoneyAsync(preAccountDeductMoneyDto);
                            }
                        }
                    }
                }
                else
                {
                    //中心的旅行社确认子订单 
                    var bodyList = await _toBodyRepository.GetAll().Where(o => o.TOHeaderId == dto.TOHeaderId).ToListAsync();
                    bodyList.ForEach(m => m.OrderState = OrderState.TradeSuccess);

                    //中心的旅行社预订子订单
                    var bodyPreList = await _toBodyPreRepository.GetAll().Where(o => o.TOHeaderPreId == dto.TOHeaderId).ToListAsync();
                    bodyPreList.ForEach(m => m.OrderState = OrderState.TradeSuccess);

                    //窗口取旅行社票时，更新预订单出票状态（旅行社站点需要看订单状态）
                    var toheaderPre = await _toHeaderPreRepository.GetAll().FirstOrDefaultAsync(m => m.Id == dto.TOHeaderId);
                    toheaderPre.MainOrderState = MainOrderState.Warranted;
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// 接收同步修改数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> OrderModify(OrderModifyDto dto)
        {
            switch (dto.ModifyType)
            {
                case ModifyType.ModifyPid:
                    await UpdatePidAsync(dto.PidData);
                    break;
                case ModifyType.ModifyPhone:
                    await UpdatePhoneAsync(dto.PhoneData);
                    break;
                case ModifyType.ModifyPlanDate:
                    await UpdatePlandateAsync(dto.ValidStartDateData);
                    break;
            }
            return Result.Ok();

        }

        /// <summary>
        /// 同步他园票
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> OtherTicketSend(OtherTicketSendDto dto)
        {
            var entity = dto.MapTo<NonGroupTicket>();
            entity.SyncTicketType = SyncTicketType.OtherTicket;
            await _nonGroupTicketRepository.InsertAsync(entity);
            return Result.Ok();
        }

        ///// <summary>
        ///// 接收中心同步过来的订单数据
        ///// </summary>
        ///// <param name="dto"></param>
        ///// <returns></returns>
        //public async Task<Result> SendOrderFromParkApi(OrderSendDto dto)
        //{
        //    //调用公园API接口
        //    var syncParkId = dto.OrderInfo.TOBodies.First().ParkId;
        //    var sync = await _syncParkRepository.GetAll().FirstAsync(o => o.ParkId == syncParkId);
        //    var uri = new Uri(sync.SyncUrl);

        //    //通过公园API接口同步中心确认的订单信息到公园
        //    var response = await WebApiHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/OrderDetail/SyncConfirmOrderToParkAsync", JsonConvert.SerializeObject(dto));
        //    var result = JsonConvert.DeserializeObject<Result>(response);

        //    return result;
        //}

        /// <summary>
        /// 接收中心同步过来的订单数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> OrderSend(OrderSendDto dto)
        {
            //导入订单信息（主订单、子订单、用户）
            var toHeaderEntity = dto.OrderInfo.MapTo<TOHeader>();

            await _toHeaderRepository.InsertAsync(toHeaderEntity);

            //导入电子票（电子票、凭证）
            foreach (var ticket in dto.TicketsInfo)
            {
                var toTicketEntity = ticket.MapTo<TOTicket>();
                await _toTicketRepository.InsertAsync(toTicketEntity);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 接收中心推送过来的取消订单确认的请求
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> OrderCancelConfirmAsync(OrderCancelConfirmDto dto)
        {
            //当公园网络中断时，有可能推送过来的订单还在公园的redis中（公园还没生成订单），这时中心取消确认时需要返回错误给中心，避免中心成功后可以再次确认订单发送第二次请求
            //从而导致中心的redis中有相同的两条推送到公园的订单数据而报主键重复的错误
            var tohead = await _toHeaderRepository.GetAllIncluding(m => m.TOBodies).FirstOrDefaultAsync(m => m.Id == dto.TOHeaderId);
            if (tohead == null)
                return Result.FromError("公园订单不存在，不允许被取消");

            if (tohead.TOBodies.Any(m => m.OrderState == OrderState.TradeSuccess))
                return Result.FromError("订单已消费，不允许取消");

            //把改订单删除，中心再重新确认后再生成新的订单信息
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                await _toBodyRepository.DeleteAsync(m => m.TOHeaderId == dto.TOHeaderId);
                await _groupInfoRepository.DeleteAsync(m => m.Id == tohead.GroupInfoId);
                await _toHeaderRepository.DeleteAsync(dto.TOHeaderId);

                await UnitOfWorkManager.Current.SaveChangesAsync();
            }

            return Result.Ok();
        }

        /// <summary>
        /// 接收中心同步过来的冻结/解冻OTA订单数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result> OrderFreezeOrUnFreezeAsync(FreezeOrderDto dto)
        {
            var tohead = await _toHeaderRepository.FirstOrDefaultAsync(dto.TOHeaderId);

            if (tohead == null)
                return Result.FromCode(ResultCode.NoRecord);

            switch (dto.DataSyncType)
            {
                case DataSyncType.OrderFreeze:
                    FreezeOrder(tohead);
                    break;
                case DataSyncType.OrderUnFreeze:
                    UnFreezeOrder(tohead);
                    break;
            }

            return Result.Ok();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 修改身份证
        /// </summary>
        /// <param name="data"></param>
        private async Task UpdatePidAsync(ICollection<ModifyPid> data)
        {
            foreach (var modifyPid in data)
            {
                //更新用户信息表pid
                await _toBodyRepository.UpdateAsync(modifyPid.TOBodyId, p => Task.FromResult(p.Customer.Pid = modifyPid.Pid));
                //更新凭证Pid
                var toVoucher = await _toVoucherRepository.FirstOrDefaultAsync(p => p.TOBodyId == modifyPid.TOBodyId);
                await _toVoucherRepository.UpdateAsync(toVoucher.Id, p => Task.FromResult(p.Pid = modifyPid.Pid));
            }
        }

        /// <summary>
        /// 修改电话号码
        /// </summary>
        /// <param name="data"></param>
        private async Task UpdatePhoneAsync(ICollection<ModifyPhone> data)
        {
            foreach (var modifyPhone in data)
            {
                await _toBodyRepository.UpdateAsync(modifyPhone.TOBodyId, p => Task.FromResult(p.Customer.PhoneNumber = modifyPhone.Phone));
            }
        }

        /// <summary>
        /// 修改预订日期
        /// </summary>
        /// <param name="data"></param>
        private async Task UpdatePlandateAsync(ModifyValidStartDate data)
        {
            await _toHeaderRepository.UpdateAsync(data.TOHeaderId, p => Task.FromResult(p.ValidStartDate = data.ValidStartDate));
        }

        /// <summary>
        /// 冻结子订单和电子票
        /// </summary>
        /// <param name="tohead"></param>
        private void FreezeOrder(TOHeader tohead)
        {
            //冻结子订单
            EnumerableExtensions.ForEach(tohead.TOBodies, m => m.OrderState = OrderState.Freezon);

            //冻结电子票
            var bodyIds = tohead.TOBodies.Select(m => m.Id);

            var tovoucherIds = _toVoucherRepository.AsNoTracking().Where(m => bodyIds.Contains(m.TOBodyId)).Select(m => m.Id).ToList();

            var totickets = _toTicketRepository.GetAll().Where(m => tovoucherIds.Contains(m.TOVoucherId)).ToList();

            totickets.ForEach(m => m.TicketSaleStatus = TicketSaleStatus.Freezon);

            foreach (var toticket in totickets)
            {
                //清掉验票的缓存
                var ticketCheckCacheDto = new TicketCheckCacheDto
                {
                    Key = toticket.Id
                };
                DataSyncInput dataSyncInput = new DataSyncInput()
                {
                    SyncData = JsonConvert.SerializeObject(ticketCheckCacheDto),
                    SyncType = DataSyncType.TicketCheckCacheClear
                };
                _dataSyncManager.UploadDataToTargetPark(toticket.ParkId, dataSyncInput);
            }
        }

        /// <summary>
        /// 解冻子订单和电子票
        /// </summary>
        /// <param name="toheader"></param>
        private void UnFreezeOrder(TOHeader toheader)
        {
            EnumerableExtensions.ForEach(toheader.TOBodies, m => m.OrderState = OrderState.WaitCost);

            //冻结电子票
            var bodyIds = toheader.TOBodies.Select(m => m.Id);

            var tovoucherIds = _toVoucherRepository.AsNoTracking().Where(m => bodyIds.Contains(m.TOBodyId)).Select(m => m.Id).ToList();

            var totickets = _toTicketRepository.GetAll().Where(m => tovoucherIds.Contains(m.TOVoucherId)).ToList();

            totickets.ForEach(m => m.TicketSaleStatus = TicketSaleStatus.Valid);

            foreach (var toticket in totickets)
            {
                //清掉验票的缓存
                var ticketCheckCacheDto = new TicketCheckCacheDto
                {
                    Key = toticket.Id
                };
                DataSyncInput dataSyncInput = new DataSyncInput()
                {
                    SyncData = JsonConvert.SerializeObject(ticketCheckCacheDto),
                    SyncType = DataSyncType.TicketCheckCacheClear
                };
                _dataSyncManager.UploadDataToTargetPark(toticket.ParkId, dataSyncInput);
            }
        }

        #endregion
    }
}

