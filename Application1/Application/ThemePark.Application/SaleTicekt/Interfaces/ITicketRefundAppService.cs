using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.Refund.Dto;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt
{

    public interface ITicketRefundAppService : IApplicationService
    {

        Task<Result<IList<TicketRefundDetailDto>>> SearchTickets4RefundAsync(string barcodeBegin, string barcodeEnd);

        /// <summary>
        /// 根据条码查询取票信息
        /// </summary>
        /// <param name="barcode">条码</param>
        /// <returns></returns>
        Task<Result<TicketRefundDetailDto>> SearchTicket4RefundAsync(string barcode);

        /// <summary>
        /// 增加退票记录
        /// </summary>
        /// <returns></returns>
        Task<Result> AddTicketRefundAsync(List<RefundTicketInput> inputs, int terminalId, int parkId);
    }
}
