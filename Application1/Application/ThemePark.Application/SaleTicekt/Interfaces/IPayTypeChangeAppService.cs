using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    public interface IPayTypeChangeAppService: IApplicationService
    {
        /// <summary>
        /// 根据其中一条码获取整笔交易的售票详情
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        Task<Result<List<GetSaleDetailDto>>> GetSaleDetail(string barcode);


        /// <summary>
        /// 支付方式更改
        /// </summary>
        /// <returns></returns>
        Task<Result> PayTypeChange(PayTypeChangeInput input);
    }
}
