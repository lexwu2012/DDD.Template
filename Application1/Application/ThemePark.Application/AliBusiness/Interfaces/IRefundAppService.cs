using Abp.Application.Services;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness.Interfaces
{
    /// <summary>
    /// 退款应用服务接口
    /// </summary>
    public interface IRefundAppService : IApplicationService
    {
        /// <summary>
        /// 当消费者在淘宝平台发起退款并且退款成功后，淘宝会通知码商系统订单已经退款成功。码商系统在收到退款成功通知后需要给淘宝收到通知的反馈，
        /// 同时可以根据自己的业务需要做退款成功后的处理，例如：作废码等。注意，码商对接此接口需保持幂等，若已处理过退款成功请求，请直接返回成功。
        /// </summary>
        /// <param name="dto"></param>
        Result HandleRefundBusiness(RefundNotificationDto dto);
    }
}
