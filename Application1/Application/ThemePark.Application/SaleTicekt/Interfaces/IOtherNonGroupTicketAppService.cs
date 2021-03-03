using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    /// <summary>
    /// 他园票应用服务接口
    /// </summary>
    public interface IOtherNonGroupTicketAppService: IApplicationService
    {
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        Task<Result<string>> UpdateGroupTicketToInvalidAndReturnTradeIdAsync(string barCode);
    }
}
