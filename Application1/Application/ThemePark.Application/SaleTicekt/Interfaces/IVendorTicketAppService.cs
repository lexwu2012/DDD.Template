using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    public interface IVendorTicketAppService: IApplicationService
    {

        /// <summary>
        /// 本公园自助售票机预订票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<List<SearchVendorOrderDto>>> SearchVendorTicket(SearchVendorTicketOrderInput input);


        /// <summary>
        /// 本公园自助售票机预订票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<List<SearchVendorOrderDto>>> SearchOtherParkVendorTicket(SearchOtherParkVendorOrderInput input);


        /// <summary>
        /// 自助售票机取票
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qty"></param>
        /// <param name="localParkId"></param>
        /// <returns></returns>
        Task<Result<List<PrintInfo>>> TakeVendorTicket(ToTicketInput input,int qty,int localParkId);


        /// <summary>
        /// 售票机买本公园票
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<string>> AddVendorTicketAsync(AddVendorTicketInput input);


        /// <summary>
        /// 新增他园票购票记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<string>> AddOtherParkVendorTicketAsync(AddOtherParkVendorTicketInput input);

    }
}
