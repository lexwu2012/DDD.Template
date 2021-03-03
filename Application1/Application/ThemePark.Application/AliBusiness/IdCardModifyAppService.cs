using System;
using System.Linq;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Json;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Helper;
using ThemePark.Application.AliBusiness.Interfaces;
using ThemePark.Core.AliPartner;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness
{
    /// <summary>
    /// 身份证更改应用服务
    /// </summary>
    public class IdCardModifyAppService : ThemeParkAppServiceBase, IIdCardModifyAppService
    {
        #region Fields

        private readonly IRepository<TmallOrderDetail, string> _tmallOrderDetailRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// cotr
        /// </summary>
        public IdCardModifyAppService(IRepository<TmallOrderDetail, string> tmallOrderDetailRepository)
        {
            _tmallOrderDetailRepository = tmallOrderDetailRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 由于买家错填导致身份证信息错误，消费者在淘宝平台修改身份证信息后，淘宝会通知码商系统最新的订单信息。 
        /// 码商系统在收到修改身份证通知后，需要给淘宝收到通知的反馈，同时码商系统需要更新自身系统记录的身份证信息。
        /// </summary>
        [DisableAuditing]
        public Result HandleIdCardBusiness(ModifyIdCardNotificationDto modifyIdNotificationDto)
        {
            var tmallOrderDetail = _tmallOrderDetailRepository.GetAll().FirstOrDefault(m => m.Id == modifyIdNotificationDto.OuterId);
            if (tmallOrderDetail == null)
            {
                Logger.Error("本地没有查询到:" + modifyIdNotificationDto.OuterId + "订单信息");
                return Result.FromError("本地没有查询到:" + modifyIdNotificationDto.OuterId + "订单信息");
            }

            var result = AliBusinessHelper.ChangeIdCardInFt(modifyIdNotificationDto, tmallOrderDetail);

            if (result?.ResultStatus == ResultState.Ok)
            {
                //本地数据更改
                tmallOrderDetail.IdCard = modifyIdNotificationDto.IdCard;
                Logger.Info("订单号为：" + tmallOrderDetail.Id + "修改身份证成功");
                return Result.Ok();
            }
            //else if (result?.ResultStatus == ResultState.InvalidToken)
            //{
            //    AliBusinessHelper.TokenEndTime = DateTime.Now.AddHours(-1);
            //}
            Logger.Error("订单号为：" + tmallOrderDetail.Id + "修改身份证失败，原因为：" + result?.Message);
            return Result.FromError("修改身份证信息失败，原因为：" + result?.Data.ToJsonString());
        }

        #endregion

        #region Private Methods


        #endregion
    }
}
