using Abp.Application.Services;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness.Interfaces
{
    /// <summary>
    /// 重发码应用服务接口
    /// </summary>
    public interface IResendAppService : IApplicationService
    {
        /// <summary>
        /// 重发码业务
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        void HandleResendBusiness(SendAndResendNotificationDto dto);
    }
}
