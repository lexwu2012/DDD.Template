using Abp.Application.Services;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness.Interfaces
{
    /// <summary>
    /// 身份证更改应用服务接口
    /// </summary>
    public interface IIdCardModifyAppService : IApplicationService
    {
        /// <summary>
        /// 由于买家错填导致身份证信息错误，消费者在淘宝平台修改身份证信息后，淘宝会通知码商系统最新的订单信息。 
        /// 码商系统在收到修改身份证通知后，需要给淘宝收到通知的反馈，同时码商系统需要更新自身系统记录的身份证信息。
        /// </summary>
        Result HandleIdCardBusiness(ModifyIdCardNotificationDto modifyIdNotificationDto);
    }
}
