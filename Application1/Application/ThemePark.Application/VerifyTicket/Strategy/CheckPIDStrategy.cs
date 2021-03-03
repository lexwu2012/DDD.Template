using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.Application.SaleCard.Interfaces;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 身份证号验票策略
    /// </summary>
    public class CheckPIDStrategy : CheckStrategyBase, ICheckStrategy
    {
        private readonly ITOVoucherAppService _orderVoucherAppService;
        private readonly IRepository<TOTicket, string> _orderTicketRepository;
        private readonly IVIPCardAppService _vipCardAppService;

        public CheckPIDStrategy(ITOVoucherAppService orderVoucherAppService,
            IRepository<TOTicket, string> orderTicketRepository,
            IVIPCardAppService vipCardAppService)
        {
            _orderVoucherAppService = orderVoucherAppService;
            _orderTicketRepository = orderTicketRepository;
            _vipCardAppService = vipCardAppService;
        }

        /// <summary>
        /// 验身份证
        /// </summary>
        /// <param name="swapCode"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> Verify(string swapCode, int terminal)
        {
            // 查询身份证所有的凭证
            var result = await _orderVoucherAppService.GetTOVoucherListAsync<TOVoucherDto>(
                new Query<TOVoucher>(p => p.Pid == swapCode));

            if (result.Code != ResultCode.NoRecord)
            {
                foreach (var voucher in result.Data)
                {
                    // 查询凭证对应的票
                    var tickets = await _orderTicketRepository.GetAllListAsync(
                        x => x.TOVoucherId == voucher.Id
                        && x.TicketSaleStatus == TicketSaleStatus.Valid
                        && x.TicketFormEnum == TicketFormEnum.ETicket
                        && x.ValidStartDate <= DateTime.Today
                        && DbFunctions.AddDays(x.ValidStartDate, x.ValidDays) >= DateTime.Today);

                    if (CheckIsMultiOrYearCard(tickets))
                        return Failed(swapCode, VerifyType.InvalidTicket, "需在窗口兑换");

                    if (tickets.Count > 0)
                    {
                        var barcode = tickets.Select(ticket => ticket.Id).First();

                        ////先通过缓存检票
                        //Result<VerifyDto> resultCache;
                        //using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckCacheStrategy>())
                        //{
                        //    resultCache = await strategy.Object.Verify(barcode, terminal);
                        //}

                        //if (resultCache != null)
                        //    return resultCache;

                        //return await IocManager.Instance.Resolve<CheckBarcodeStrategy>().Verify(barcode, terminal);
                        //先通过缓存检票
                        Result<VerifyDto> resultCache;
                        using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckCacheStrategy>())
                        {
                            resultCache = await strategy.Object.Verify(barcode, terminal);
                        }
                        //缓存不存在
                        if (resultCache == null)
                        {
                            var verifyResult = await IocManager.Instance.Resolve<CheckBarcodeStrategy>()
                                .Verify(barcode, terminal);
                            if (verifyResult.Success)
                            {
                                return verifyResult;
                            }
                        }
                        else if (resultCache.Success)
                        {
                            return resultCache;
                        }
                    }
                }
            }

            // 查询身份证关联的VIP卡
            var vipCard = await _vipCardAppService.GetCardInfoByPidAsync(swapCode);
            if (vipCard != null && vipCard.Data != null && vipCard.Data.Count > 0)
            {
                string vipNo = vipCard.Data[0].IcNo.ToUpper();
                DateTime endTime = System.DateTime.Now.AddDays(-1);
                foreach (var card in vipCard.Data)
                {
                    if (endTime < Convert.ToDateTime(card.ValidDateEnd))
                    {
                        endTime = Convert.ToDateTime(card.ValidDateEnd);
                        vipNo = card.IcNo.ToUpper();
                    }
                }
                //string vipNo = vipCard.Data[0].IcNo.ToUpper();
                if (vipNo != "") // 走IC卡验票逻辑
                    return await IocManager.Instance.Resolve<CheckICNoStrategy>().Verify(vipNo, terminal);
            }

            return Failed(swapCode, VerifyType.InvalidTicket, "无效|身份证");
        }

        /// <summary>
        /// 判断是否含有年卡和套票
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        private bool CheckIsMultiOrYearCard(List<TOTicket> tickets)
        {
            if (tickets.Exists(p => p.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == Core.BasicTicketType.TicketClassMode.MultiParkTicket ||
                                    p.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == Core.BasicTicketType.TicketClassMode.YearCard ||
                                    p.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == Core.BasicTicketType.TicketClassMode.MultiYearCard))
            {
                return true;
            }
            return false;
        }
    }
}