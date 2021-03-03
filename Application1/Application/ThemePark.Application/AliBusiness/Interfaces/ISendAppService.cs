using Abp.Application.Services;
using ThemePark.Application.AliBusiness.Dto;

namespace ThemePark.Application.AliBusiness.Interfaces
{
    /// <summary>
    /// 发码应用服务接口
    /// </summary>
    public interface ISendAppService : IApplicationService
    {
        /// <summary>
        /// 处理发码流程
        /// </summary>
        /// <returns></returns>
        void HandleSendBusiness(SendAndResendNotificationDto dto);
    }
}
