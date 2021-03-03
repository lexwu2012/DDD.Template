using Abp.Application.Services;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness.Interfaces
{
    /// <summary>
    /// 维权应用服务接口
    /// </summary>
    public interface ISuitAppService : IApplicationService
    {
        /// <summary>
        /// 当消费者在淘宝平台发起维权并且维权成功后，淘宝会通知码商系统订单已经维权成功。
        /// 码商系统在收到维权成功通知后需要给淘宝收到通知的反馈，同时可以根据自己的业务需要做维权成功后的处理，例如：作废码等。
        /// 注意，码商对接此接口需保持幂等，若已处理过维权成功请求，请直接返回成功。
        /// </summary>
        Result HandleSuitBusiness(SuitNotificationDto dto);
    }
}
