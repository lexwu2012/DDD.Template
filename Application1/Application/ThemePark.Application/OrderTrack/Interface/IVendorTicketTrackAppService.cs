using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.OrderTrack.Dto;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.Trade.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.OrderTrack.Interface
{
    public interface IVendorTicketTrackAppService
    {

        /// <summary>
        /// 自助售票机取票打印追踪
        /// </summary>
        /// <returns></returns>
        Task<Result> TakeVendorTicketTrack(List<PrintInfo> result,TicketTrackInput input);

        /// <summary>
        /// 订单查询追踪
        /// </summary>
        /// <param name="result"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> SearchVendorTicketTrack(List<SearchVendorOrderDto> result, TicketTrackInput input);

        /// <summary>
        /// 自助售票机支付追踪
        /// </summary>
        /// <param name="tradeInfo"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> PayVendorTicketTrack(TradeInfoInput tradeInfo,TicketTrackInput input);

        /// <summary>
        /// 自助售票机取票打印追踪
        /// </summary>
        /// <returns></returns>
        Task<Result> SaleVendorTicketTrack(List<PrintInfo> result, TicketTrackInput input);

        /// <summary>
        /// 自助售票机异常信息追踪
        /// </summary>
        /// <param name="toHeaderId"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ErrorVendorTicketTrack(string toHeaderId, int terminalId, int parkId, string message);
    }



}
