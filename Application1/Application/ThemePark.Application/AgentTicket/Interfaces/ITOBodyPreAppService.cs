using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 预订子订单应用服务接口
    /// </summary>
    public interface ITOBodyPreAppService : IApplicationService
    {
        /// <summary>
        /// 根据订单表头Id获取订单详情
        /// </summary>
        /// <param name="query">订单头id</param>
        /// <returns></returns>
        Task<List<TDto>> GetTOBodyPreListAsync<TDto>(IQuery<TOBodyPre> query);
    }

}
