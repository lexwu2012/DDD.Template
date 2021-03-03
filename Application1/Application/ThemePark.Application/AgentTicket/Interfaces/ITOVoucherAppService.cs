using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface ITOVoucherAppService : IApplicationService
    {
       
        ///// <summary>
        ///// 根据条件查询凭证
        ///// </summary>
        ///// <typeparam name="TDto"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //Task<Result<TDto>> GetTOVoucherAsync<TDto>(Query<TOVoucher> query);

        /// <summary>
        /// 根据条件查询凭证
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<Result<List<TDto>>> GetTOVoucherListAsync<TDto>(Query<TOVoucher> query);

    }
}
