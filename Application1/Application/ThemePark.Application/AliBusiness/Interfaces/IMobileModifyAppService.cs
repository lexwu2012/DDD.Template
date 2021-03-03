using Abp.Application.Services;
using ThemePark.Application.AliBusiness.Dto;

namespace ThemePark.Application.AliBusiness.Interfaces
{
    /// <summary>
    /// 手机号修改应用层服务接口
    /// </summary>
    public interface IMobileModifyAppService : IApplicationService
    {
        /// <summary>
        /// 在没有收到凭证码等情况下，买家可能会发起修改手机号码的操作，淘宝会通知码商系统进行修改手机号码操作，码商系统在收到通知后，需要给淘宝收到通知的反馈。
        ///  同时码商系统在收到淘宝的修改手机号码通知后，根据自身业务进行修改手机号码的一些操作，
        /// 包括但不限于给消费者的新手机号码发送相应的凭证码（需要根据用户通知类型决定是否给消费者户手机发码），回调淘宝平台提供的重新发码回调接口等。
        /// </summary>
        void HandleMobileModifyBusiness(ModifyMobileNotificationDto modifyIdNotificationDto);
    }
}
