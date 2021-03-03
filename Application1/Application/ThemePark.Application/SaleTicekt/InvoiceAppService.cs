using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Core.ParkSale;
using ThemePark.Core.ParkSale.DomainServiceInterfaces;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using System.Data.Entity;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 发票应用服务层
    /// </summary>
    public class InvoiceAppService : ThemeParkAppServiceBase, IInvoiceAppService
    {
        #region Fields
        private readonly IInvoiceDomainService _invoiceDomainService;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="invoiceDomainService"></param>
        /// <param name="invoiceRepository"></param>
        /// <param name="invoiceCodeRepository"></param>
        public InvoiceAppService(IInvoiceDomainService invoiceDomainService, IRepository<Invoice, long> invoiceRepository,
            IRepository<InvoiceCode> invoiceCodeRepository)
        {
            _invoiceDomainService = invoiceDomainService;
            _invoiceRepository = invoiceRepository;
            _invoiceCodeRepository = invoiceCodeRepository;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 根据表达式获取发票
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<Invoice> GetInvoiceAsync(Expression<Func<Invoice, bool>> exp)
        {
            return await _invoiceDomainService.GetInvoiceAsync(exp);
        }

        /// <summary>
        /// 获取发票号
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="invoiceCode"></param>
        /// <returns></returns>
        public async Task<Result<InvoiceInput>> GetAvailableInvoiceNo(int terminalId, string invoiceCode)
        {
            //找出当前机器和发票代码下的发票号
            var result = await _invoiceRepository.GetAllListAsync(p => p.TerminalId == terminalId && p.InvoiceCode == invoiceCode
                && DbFunctions.DiffDays(p.CreationTime, DateTime.Now) == 0);

            var invoiceCodeEntity = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == invoiceCode);

            Invoice invoice;
            long maxInvoiceNo;

            //发票号递增
            if (invoiceCodeEntity.InvoiceNumIsIncrease)
            {
                invoice = result.OrderByDescending(m => m.CreationTime).ThenByDescending(m => m.InvoiceNo).FirstOrDefault();

                if (invoice == null)
                    return Result.FromCode<InvoiceInput>(ResultCode.NoRecord);

                maxInvoiceNo = long.Parse(invoice.InvoiceNo) + 1;
            }
            else
            {
                invoice = result.OrderByDescending(m => m.CreationTime).ThenBy(m => m.InvoiceNo).FirstOrDefault();

                if (invoice == null)
                    return Result.FromCode<InvoiceInput>(ResultCode.NoRecord);

                //发票号递减
                maxInvoiceNo = long.Parse(invoice.InvoiceNo) - 1;
            }

            var availableInvoiceNo = maxInvoiceNo.ToString(new string('0', invoice.InvoiceNo.Length));
            var data = new InvoiceInput() { InvoiceNo = availableInvoiceNo, InvoiceCode = invoice.InvoiceCode };
            return Result.FromData(data);
        }

        /// <summary>
        /// 根据发票代码，发票号检查是否存在重复的发票号
        /// </summary>
        /// <param name="invoiceCode">发票代码</param>
        /// <param name="invoiceNo">发票号</param>
        /// <returns></returns>
        public bool CheckIfExistedDuplicateInvoice(string invoiceCode, string invoiceNo)
        {
            return _invoiceRepository.GetAll().Any(m => m.InvoiceCode == invoiceCode
                                                    && m.InvoiceNo == invoiceNo
                                                    //&& DbFunctions.DiffDays(m.CreationTime, DateTime.Now) == 0
                                                    //&& m.TerminalId == terminalId
                                                    && m.IsActive);
        }

        /// <summary>
        /// 根据发票代码，发票号检查是否存在重复的发票号或者负数
        /// </summary>
        /// <param name="invoiceCode">发票代码</param>
        /// <param name="invoiceNo">发票号</param>
        /// <param name="ticketCount">票的数量</param>
        /// <param name="invoiceNumIsIncrease">发票是否递增/递减</param>
        /// <returns></returns>
        public bool CheckIfExisteInValidOrDuplicateInvoice(string invoiceCode, string invoiceNo, int ticketCount, bool invoiceNumIsIncrease)
        {
            bool existedOrInvalid = false;
            var checkInvoiceNo = invoiceNo;
            if (invoiceNumIsIncrease)
            {
                for (var i = 0; i < ticketCount; i++)
                {
                    existedOrInvalid = _invoiceRepository.GetAll().Any(m => m.InvoiceCode == invoiceCode
                                                         && m.InvoiceNo == checkInvoiceNo);
                    if (existedOrInvalid)
                        break;

                    var invoice = long.Parse(checkInvoiceNo);
                    invoice++;
                    checkInvoiceNo = invoice.ToString(new string('0', invoiceNo.Length));
                }
            }
            else
            {
                for (var i = 0; i < ticketCount; i++)
                {
                    existedOrInvalid = _invoiceRepository.GetAll().Any(m => m.InvoiceCode == invoiceCode
                                                         && m.InvoiceNo == checkInvoiceNo);
                    if (existedOrInvalid)
                        break;

                    var invoice = long.Parse(checkInvoiceNo);
                    invoice--;

                    //预防返回负数
                    if (invoice < 0)
                    {
                        existedOrInvalid = true;
                        break;
                    }

                    checkInvoiceNo = invoice.ToString(new string('0', invoiceNo.Length));
                }

            }
            return existedOrInvalid;
        }

        /// <summary>
        /// 窗口取其他公园网络订单,生成其他公园网络单发票号
        /// </summary>
        /// <param name="input"></param>
        /// <param name="barcodes"></param>
        /// <param name="invoiceUsed"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> MakeUpWebTicketInvoice(InvoiceInput input, List<string> barcodes, int invoiceUsed, int terminalId)
        {
            var invoiceCode = await _invoiceCodeRepository.FirstOrDefaultAsync(p=>p.Code == input.InvoiceCode);
            if (invoiceCode == null)
                return Result.FromCode(ResultCode.Fail);
            var invoiceNo = invoiceCode.InvoiceNumIsIncrease
                ? long.Parse(input.InvoiceNo) + invoiceUsed
                : long.Parse(input.InvoiceNo) - invoiceUsed;
            foreach (var barcode in barcodes)
            {
                //新增缺失的发票记录
                await _invoiceRepository.InsertAsync(new Invoice()
                {
                    InvoiceNo = invoiceCode.InvoiceNumIsIncrease ? invoiceNo++.ToString(new string('0', input.InvoiceNo.Length)) : invoiceNo--.ToString(new string('0', input.InvoiceNo.Length)),
                    InvoiceCode = input.InvoiceCode,
                    Barcode = barcode,
                    TerminalId = terminalId,
                    IsActive = true
                });
            }
            return Result.Ok();
        }


        /// <summary>
        /// 获取所有发票号
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TDto>> GetInvoiceListAsync<TDto>(IQuery<Invoice> query)
        {
            return await _invoiceRepository.AsNoTracking().ToListAsync<Invoice, TDto>(query);
        }
        #endregion
    }
}
