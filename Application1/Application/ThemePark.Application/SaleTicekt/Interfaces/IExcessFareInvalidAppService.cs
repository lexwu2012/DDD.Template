using Abp.Application.Services;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    /// <summary>
    /// 作废票应用接口
    /// </summary>
    public interface IExcessFareInvalidAppService : IApplicationService
    {
        /// <summary>
        /// 添加作废票记录
        /// </summary>
        /// <returns></returns>
        Task<Result> AddExcessFareInvalid(ExcessFareInvalidDto dto);
    }
}
