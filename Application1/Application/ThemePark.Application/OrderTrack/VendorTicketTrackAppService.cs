using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Application.OrderTrack.Dto;
using ThemePark.Application.OrderTrack.Interface;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.Trade.Dto;
using ThemePark.Common;
using ThemePark.Core.BasicData;
using ThemePark.Core.OrderTrack;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.OrderTrack
{
    public class VendorTicketTrackAppService : ThemeParkAppServiceBase, IVendorTicketTrackAppService
    {

        private readonly IRepository<VendorTicketTrack> _vendorTicketTrackRepository;
        private readonly IRepository<Park> _parkRepository;

        public VendorTicketTrackAppService(IRepository<VendorTicketTrack> vendorTicketTrackRepository, IRepository<Park> parkRepository)
        {
            _vendorTicketTrackRepository = vendorTicketTrackRepository;
            _parkRepository = parkRepository;
        }


        /// <summary>
        /// 自助售票机取票打印追踪
        /// </summary>
        /// <param name="result"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> TakeVendorTicketTrack(List<PrintInfo> result, TicketTrackInput input)
        {
            //获取所有主订单号
            var toHeaderIds = new List<string>();
            result.Select(p => p.TicketContent.OrderId).ToList().Distinct().ForEach(p => toHeaderIds.Add(p.Substring(0, 18)));
            foreach (var toheaderId in toHeaderIds)
            {
                //提取该toheaderId的printInfo
                var info = result.Where(p => p.TicketContent.OrderId.IndexOf(toheaderId, StringComparison.Ordinal) >= 0).ToList();
             
                //取票详情 全价票1张、儿童票2张
                var ticketDetail = "";
                info.GroupBy(p => p.TicketContent.TicketMarker).ToList().ForEach(p => ticketDetail += $"{p.Key}{p.Count()}张、");
                ticketDetail = ticketDetail.Substring(0, ticketDetail.Length - 1);

                //条码：78500000121、78500000122
                string barcodes = "";
                info.Select(p => p.TicketContent.Barcode).ToList().Distinct().ForEach(p => barcodes += $"{p + "、"}");
                barcodes = barcodes.Substring(0, barcodes.Length - 1);

                //追踪信息 取11002111000013,全价票1张、儿童票1张，条码78500000121、78500000122
                var log = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：取{toheaderId}，{ticketDetail}，条码{barcodes}";

                var track = await _vendorTicketTrackRepository.FirstOrDefaultAsync(p => p.TOHeaderId == toheaderId && p.TerminalId == input.TerminalId);
                //没找到追踪记录，查找订单环节缺失
                if (track == null)
                {
                    await ErrorVendorTicketTrack(toheaderId, input.TerminalId, input.ParkId, "系统错误，缺失订单查询环节追踪");
                    return Result.FromCode(ResultCode.Fail);
                }

                await _vendorTicketTrackRepository.UpdateAsync(track.Id, p => Task.FromResult(p.TrackLog += "|" + log));
            }

            await UnitOfWorkManager.Current.SaveChangesAsync();
            return Result.Ok();

        }


        /// <summary>
        /// 订单查询追踪
        /// </summary>
        /// <param name="result"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> SearchVendorTicketTrack(List<SearchVendorOrderDto> result, TicketTrackInput input)
        {
            var toHeaderIds = result.Select(p => p.TOHeaderId).ToList();
            foreach (var toHeaderId in toHeaderIds)
            {
                var infos = result.Where(p => p.TOHeaderId == toHeaderId).ToList();
                var entity = await _vendorTicketTrackRepository.FirstOrDefaultAsync(p => p.TOHeaderId == toHeaderId&&p.TerminalId == input.TerminalId);
                if (entity != null)
                {
                    await _vendorTicketTrackRepository.UpdateAsync(entity.Id, p => Task.FromResult(p.TrackLog += $"|{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：查询订单{toHeaderId}"));
                    return Result.Ok();
                }

                //订单详情
                var orderDetail = "";
                infos.ForEach(p => orderDetail += $"{p.SaleTicketClassName} {p.Qty}张、");
                orderDetail = orderDetail.Substring(0, orderDetail.Length - 1);

                //日志格式：时间：查询梦话王国网络订单，订单号：TOHeaderId,包含子订单：TOBodyId 全价票2张
                var log = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：查询订单{toHeaderId}，{orderDetail}";

                await _vendorTicketTrackRepository.InsertAsync(new VendorTicketTrack()
                {
                    TOHeaderId = toHeaderId,
                    TerminalId = input.TerminalId,
                    ParkId = infos.First().ParkId,
                    TrackLog = log
                });
            }

            await UnitOfWorkManager.Current.SaveChangesAsync();

            return Result.Ok();
        }


        /// <summary>
        /// 自助售票机支付追踪
        /// </summary>
        /// <param name="tradeInfo"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> PayVendorTicketTrack(TradeInfoInput tradeInfo, TicketTrackInput input)
        {
            //公园
            var park = await _parkRepository.GetAsync(input.ParkId);

            //日志格式：时间:支付梦幻王国订单,交易号：tradeNo，总金额:100，微信支付，支付码：12121212
            var log = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：支付{park.ParkName}订单，交易号{input.TradeinfoId}，总金额{tradeInfo.Amount}，" +
                      $"{tradeInfo.TradeInfoDetails.First().PayModeId.DisplayName()}，支付码{tradeInfo.AuthCode}";

            await _vendorTicketTrackRepository.InsertAsync(new VendorTicketTrack()
            {
                TradeinfoId = input.TradeinfoId,
                TerminalId = input.TerminalId,
                ParkId = input.ParkId,
                TrackLog = log
            });

            await UnitOfWorkManager.Current.SaveChangesAsync();
            return Result.Ok();
        }


        /// <summary>
        /// 售票追踪
        /// </summary>
        /// <param name="result"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> SaleVendorTicketTrack(List<PrintInfo> result, TicketTrackInput input)
        {
            //取票详情
            var ticketDetail = "";
            result.GroupBy(p => p.TicketContent.TicketMarker).ToList().ForEach(p => ticketDetail += $"{p.Key}{p.Count()}张、");
            ticketDetail = ticketDetail.Substring(0, ticketDetail.Length - 1);

            //条码：78500000121、78500000122
            string barcodes = "";
            result.Select(p => p.TicketContent.Barcode).ToList().Distinct().ForEach(p => barcodes += $"{p + "、"}");
            barcodes = barcodes.Substring(0, barcodes.Length - 1);

            //追踪信息 时间：售梦幻王国票，交易号:TOHeaderId,取子订单TOBodyId,全价票1张、儿童票2张、共3张
            var log = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：售{result.First().TicketContent.ParkName}票,交易号{input.TradeinfoId},{ticketDetail}，条码{barcodes}";

            var track = await _vendorTicketTrackRepository.FirstOrDefaultAsync(p => p.TradeinfoId == input.TradeinfoId && p.TerminalId == input.TerminalId && p.ParkId == input.ParkId);
            if (track == null)
            {
                await ErrorVendorTicketTrack(input.TOHeaderId, input.TerminalId, input.ParkId, "系统错误，无支付记录");
                return Result.FromCode(ResultCode.Fail);
            }

            await _vendorTicketTrackRepository.UpdateAsync(track.Id, p => Task.FromResult(p.TrackLog += "|" + log));
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 订单异常记录
        /// </summary>
        /// <param name="toHeaderId"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ErrorVendorTicketTrack(string toHeaderId,int terminalId,int parkId,string message)
        {
            await _vendorTicketTrackRepository.InsertAsync(new VendorTicketTrack()
            {
                TOHeaderId = toHeaderId,
                TerminalId = terminalId,
                ParkId = parkId,
                IsError = true,
                ErrorMessage = message
            });
        }
    }
}
