using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// TOBody
    /// </summary>
    public interface ITOBodyAppService : IApplicationService
    {
        /// <summary>
        /// 根据订单表头Id获取订单详情
        /// </summary>
        /// <param name="query">订单头id</param>
        /// <returns></returns>
        Task<List<TDto>> GetTOBodyListAsync<TDto>(IQuery<TOBody> query);

        /// <summary>
        /// 根据主订单Id删除子订单
        /// </summary>
        /// <param name="toHeadId"></param>
        /// <returns></returns>
        Task<Result> DeleteTOBodyListAsync(string toHeadId);

    }
}
