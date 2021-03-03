using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Core.BasicData;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Infrastructure.Application;
using AutoMapper.QueryableExtensions;
using ThemePark.Infrastructure.EntityFramework;
using System.Data.Entity;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 发票代码
    /// </summary>
    public class InvoiceCodeAppService : IInvoiceCodeAppService
    {
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;

        /// <summary>
        /// 
        /// </summary>
        public InvoiceCodeAppService(IRepository<InvoiceCode> invoiceCodeRepository)
        {
            _invoiceCodeRepository = invoiceCodeRepository;
        }

        /// <summary>
        /// 保存/修改发票代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> SaveInvoiceCodeAsync(InvoiceCodeInput input)
        {

            if (string.IsNullOrWhiteSpace(input.Code))
            {
                return Result.FromError("发票代码不能为空");
            }

            var invoiceCode = await _invoiceCodeRepository.FirstOrDefaultAsync(p => p.Code == input.Code);
            if (invoiceCode != null)
            {
                invoiceCode.Company = input.Company;
                invoiceCode.LastModificationTime = DateTime.Now;
                invoiceCode.IsActive = input.IsActive;
                invoiceCode.IsUpload = input.IsUpload;
                invoiceCode.Remark = input.Remark;
                invoiceCode.SaleModuel = input.SaleModuel;
                invoiceCode.InvoiceNumIsIncrease = input.InvoiceNumIsIncrease;
            }
            else
            {
                invoiceCode = new InvoiceCode()
                {
                    Code = input.Code,
                    Company = input.Company,
                    CreationTime = DateTime.Now,
                    IsActive = input.IsActive,
                    IsUpload = input.IsUpload,
                    Remark = input.Remark,
                    SaleModuel = input.SaleModuel,
                    InvoiceNumIsIncrease = input.InvoiceNumIsIncrease
                };
            }

            await _invoiceCodeRepository.InsertOrUpdateAsync(invoiceCode);
            return Result.Ok();
        }

        /// <summary>
        /// 获取发票代码
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IList<InvoiceCodeDto>>> GetInvoiceCodeAsync()
        {
            var query = _invoiceCodeRepository.AsNoTracking();
            var data = await query.ProjectTo<InvoiceCodeDto>().ToListAsync();

            //var  salemodel = SaleModuelType.Group | SaleModuelType.Internet| SaleModuelType.Visitor;
            //salemodel = (SaleModuelType)Enum.Parse(typeof(SaleModuelType), "3");
            //salemodel.HasFlag
            //salemodel = (SaleModuelType)Enum.Parse(typeof(SaleModuelType), "3");
            //var xx=week.ToString();

            return new Result<IList<InvoiceCodeDto>>(data);
        }

        /// <summary>
        /// 获取发票代码(销售页面)
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IList<InvoiceCodeSaleDto>>> GetInvoiceCodeBySale(SaleModuelType saleModuel)
        {
            var query = _invoiceCodeRepository.AsNoTracking().Where(p => p.SaleModuel.HasFlag(saleModuel) && p.IsActive).OrderByDescending(p => p.CreationTime);

            var data = await query.ProjectTo<InvoiceCodeSaleDto>().ToListAsync();

            return new Result<IList<InvoiceCodeSaleDto>>(data);
        }





    }
}
