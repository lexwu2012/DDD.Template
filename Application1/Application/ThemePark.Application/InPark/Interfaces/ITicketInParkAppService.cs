using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Core.InPark;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.InPark.Interfaces
{
    /// <summary>
    /// 入园记录应用服务接口
    /// </summary>
    public interface ITicketInParkAppService : IApplicationService
    {
        /// <summary>
        /// 根据条件获取入园记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetTicketInParkAsync<TDto>(IQuery<TicketInPark> query);

        /// <summary>
        /// 根据条件获取入园记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetTicketInParkListAsync<TDto>(IQuery<TicketInPark> query);
    }
}
