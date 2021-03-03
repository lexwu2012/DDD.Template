using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;

namespace ThemePark.Application.VerifyTicket.Strategy
{
    /// <summary>
    /// 二维码验证逻辑
    /// </summary>
    class CheckQRCodeStrategy : CheckStrategyBase, ICheckStrategy
    {
        private readonly ITOBodyAppService _toBodyAppService;
        private readonly ITOVoucherAppService _toVoucherAppService;
        private readonly ITOTicketAppService _toTicketAppService;
        private readonly IAgencySaleTicketClassAppService _toAgencySaleTicketClassService;

        public CheckQRCodeStrategy(ITOBodyAppService toBodyAppService, ITOVoucherAppService toVoucherAppService, ITOTicketAppService toTicketAppService, IAgencySaleTicketClassAppService toAgencySaleTicketClassService)
        {
            _toBodyAppService = toBodyAppService;
            _toVoucherAppService = toVoucherAppService;
            _toTicketAppService = toTicketAppService;
            _toAgencySaleTicketClassService = toAgencySaleTicketClassService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qRCode">二维码</param>
        /// <param name="terminal">终端号</param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> Verify(string qRCode, int terminal)
        {
            var tOBodyList = await _toBodyAppService.GetTOBodyListAsync<TOBodyDto>(new Query<TOBody>(p => p.TOHeaderId == qRCode && (p.TOHeader.ValidStartDate >= DateTime.Today || p.TOHeader.ValidEndDate >= DateTime.Today)));

            if (await CheckIsMultiTicketOrYearCard(tOBodyList))
                return Failed(qRCode, VerifyType.InvalidTicket, "需在窗口兑换");

            foreach (var toBody in tOBodyList)
            {
                //toBody.AgencySaleTicketClassId
                var toVoucherList =
                    await _toVoucherAppService.GetTOVoucherListAsync<TOVoucherDto>(
                        new Query<TOVoucher>(p => p.TOBodyId == toBody.Id));
                if(toVoucherList==null || toVoucherList.Data == null)
                {
                    return Failed(qRCode, VerifyType.InvalidTicket, "需在窗口兑换");
                }

                foreach (TOVoucherDto voucher in toVoucherList.Data)
                {
                    var toTicket = await _toTicketAppService.GetTOTicketAsync<TOTicketDto>(
                        new Query<TOTicket>(p => p.TOVoucherId == voucher.Id && p.TicketFormEnum == TicketFormEnum.ETicket));

                    if (toTicket.TicketSaleStatus == TicketSaleStatus.Valid)
                    {
                        //先通过缓存检票
                        Result<VerifyDto> result;
                        using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckCacheStrategy>())
                        {
                            result = await strategy.Object.Verify(toTicket.Id, terminal);
                        }
                        //缓存不存在
                        if (result == null)
                        {
                            return await IocManager.Instance.Resolve<CheckBarcodeStrategy>()
                                .Verify(toTicket.Id, terminal);
                        }
                        return result;
                    }
                }
            }
            return Failed(qRCode, VerifyType.InvalidTicket, "无效二维码");
        }
        /// <summary>
        /// 判断是否含有年卡和套票
        /// </summary>
        /// <param name="tOBodyList"></param>
        /// <returns></returns>
        private async Task<bool> CheckIsMultiTicketOrYearCard(List<TOBodyDto> tOBodyList)
        {
            //当订单里面含有套票时必须先取票
            foreach (var toBody in tOBodyList)
            {
                var agencySaleTicketClass =
                    await _toAgencySaleTicketClassService.GetAgencySaleTicketClassAsync<AgencySaleTicketClassDto>(
                        new Query<AgencySaleTicketClass>(p => p.Id == toBody.AgencySaleTicketClassId));

                if (agencySaleTicketClass.AgencySaleTicketClassTemplate.TicketClassMode == TicketClassMode.MultiParkTicket ||
                    agencySaleTicketClass.AgencySaleTicketClassTemplate.TicketClassMode == TicketClassMode.YearCard ||
                    agencySaleTicketClass.AgencySaleTicketClassTemplate.TicketClassMode == TicketClassMode.MultiYearCard)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
