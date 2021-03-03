using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Json;
using ThemePark.AliPartner.Constants;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Helper;
using ThemePark.Application.AliBusiness.Interfaces;
using ThemePark.Core.AliPartner;

namespace ThemePark.Application.AliBusiness
{
    /// <summary>
    /// 手机号修改应用层服务
    /// </summary>
    public class MobileModifyAppService : ThemeParkAppServiceBase, IMobileModifyAppService
    {
        #region Fields
        private readonly IRepository<TmallOrder, string> _tmallOrderRepository;
        private readonly IRepository<TmallOrderDetail, string> _tmallOrderDetailRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// cotr
        /// </summary>
        public MobileModifyAppService(IRepository<TmallOrderDetail, string> tmallOrderDetailRepository, IRepository<TmallOrder, string> tmallOrderRepository)
        {
            _tmallOrderDetailRepository = tmallOrderDetailRepository;
            _tmallOrderRepository = tmallOrderRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 在没有收到凭证码等情况下，买家可能会发起修改手机号码的操作，淘宝会通知码商系统进行修改手机号码操作，码商系统在收到通知后，需要给淘宝收到通知的反馈。
        ///  同时码商系统在收到淘宝的修改手机号码通知后，根据自身业务进行修改手机号码的一些操作，
        /// 包括但不限于给消费者的新手机号码发送相应的凭证码（需要根据用户通知类型决定是否给消费者户手机发码），回调淘宝平台提供的重新发码回调接口等。
        /// </summary>
        public void HandleMobileModifyBusiness(ModifyMobileNotificationDto modifyIdNotificationDto)
        {
            var tmallOrderDetail = _tmallOrderDetailRepository.GetAll().FirstOrDefault(m => m.Id == modifyIdNotificationDto.OuterId);

            //var tmallOrderDetail = _tmallOrderDetailRepository.GetAll().FirstOrDefault(m => m.Id == modifyIdNotificationDto.OuterId);
            if (tmallOrderDetail == null)
            {
                Logger.Error("修改手机号业务中，本地没有查询到:" + modifyIdNotificationDto.OuterId + "订单信息");
                return;
            }

            if (!string.IsNullOrEmpty(tmallOrderDetail.TicketCode))
            {
                //重发码
                var result = AliBusinessHelper.ResendECode(tmallOrderDetail.TicketCode, tmallOrderDetail.IdCard, 
                    modifyIdNotificationDto.OuterId, Convert.ToInt32(modifyIdNotificationDto.Amount), tmallOrderDetail.Token, string.Empty);

                if (!string.IsNullOrWhiteSpace(result?.RetCode) && (result.RetCode.Equals(NotificationResult.SuccessAll) || result.RetCode.Equals(NotificationResult.AlreaySend)))
                {
                    _tmallOrderRepository.Update(
                        modifyIdNotificationDto.MainOrderId ?? modifyIdNotificationDto.OuterId,
                        m => Task.FromResult(m.Mobile = modifyIdNotificationDto.Mobile));
                    Logger.Info("订单号:" + tmallOrderDetail.Id + "修改手机号后重发码成功，取票码: " + tmallOrderDetail.TicketCode);
                }
                else
                {
                    Logger.Error("订单号:" + tmallOrderDetail.Id + "修改手机号业务中重发码失败，原因为：\r\n" + result?.ToJsonString());
                }
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
