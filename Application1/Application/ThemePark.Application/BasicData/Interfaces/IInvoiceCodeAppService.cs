using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    /// <summary>
    /// 发票代码接口
    /// </summary>
    public interface IInvoiceCodeAppService : IApplicationService
    {

        /// <summary>
        /// 新增发票代码
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Result.</returns>
        Task<Result> SaveInvoiceCodeAsync(InvoiceCodeInput input);

        /// <summary>
        /// 获取发票代码
        /// </summary>
        /// <returns></returns>
        Task<Result<IList<InvoiceCodeDto>>> GetInvoiceCodeAsync();

        /// <summary>
        /// 获取发票代码（销售）
        /// </summary>
        /// <param name="saleModuel"></param>
        /// <returns></returns>
        Task<Result<IList<InvoiceCodeSaleDto>>> GetInvoiceCodeBySale(SaleModuelType saleModuel);
    }
}
