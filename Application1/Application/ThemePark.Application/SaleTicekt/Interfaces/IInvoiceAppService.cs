using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    /// <summary>
    /// 发票应用服务接口
    /// </summary>
    public interface IInvoiceAppService : IApplicationService
    {
        /// <summary>
        /// 根据表达式获取发票
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<Invoice> GetInvoiceAsync(Expression<Func<Invoice, bool>> exp);

        /// <summary>
        /// 获取发票号
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="invoiceCode"></param>
        /// <returns></returns>
        Task<Result<InvoiceInput>> GetAvailableInvoiceNo(int terminalId, string invoiceCode);

        /// <summary>
        /// 获取发票号
        /// </summary>
        /// <returns></returns>
        Task<IList<TDto>> GetInvoiceListAsync<TDto>(IQuery<Invoice> query);

        /// <summary>
        /// 根据发票代码，发票号检查是否存在当天重复的发票
        /// </summary>
        /// <param name="invoiceCode">终端号</param>
        /// <param name="invoiceNo">终端号</param>
        /// <returns></returns>
        bool CheckIfExistedDuplicateInvoice(string invoiceCode, string invoiceNo);

        /// <summary>
        /// 根据发票代码，发票号检查是否存在重复的发票号
        /// </summary>
        /// <param name="invoiceCode">发票代码</param>
        /// <param name="invoiceNo">发票号</param>
        /// <param name="ticketCount">票的数量</param>
        /// <param name="invoiceNumIsIncrease">发票是否递增/递减</param>
        /// <returns></returns>
        bool CheckIfExisteInValidOrDuplicateInvoice(string invoiceCode, string invoiceNo, int ticketCount,
            bool invoiceNumIsIncrease);

        /// <summary>
        /// 弥补窗口出其他公园网络票发票记录
        /// </summary>
        /// <returns></returns>
        Task<Result> MakeUpWebTicketInvoice(InvoiceInput input, List<string> barcodes,int invoiceUsed,int terminalId);
    }
}
