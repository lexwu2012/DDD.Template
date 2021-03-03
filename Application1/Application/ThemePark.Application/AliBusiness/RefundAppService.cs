using System;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Json;
using ThemePark.AliPartner.Constants;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Enum;
using ThemePark.Application.AliBusiness.Helper;
using ThemePark.Application.AliBusiness.Interfaces;
using ThemePark.Core.AliPartner;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness
{
    /// <summary>
    /// 退款应用服务
    /// </summary>
    public class RefundAppService : ThemeParkAppServiceBase, IRefundAppService
    {
        #region Fields
        private readonly IRepository<TmallOrderDetail, string> _tmallOrderDetailRepository;
        private readonly IRepository<TmallOrderRefund> _tmallOrderRefundrRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// cotr
        /// </summary>
        public RefundAppService(IRepository<TmallOrderRefund> tmallOrderRefundrRepository, IRepository<TmallOrderDetail, string> tmallOrderDetailRepository)
        {
            _tmallOrderRefundrRepository = tmallOrderRefundrRepository;
            _tmallOrderDetailRepository = tmallOrderDetailRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 当消费者在淘宝平台发起退款并且退款成功后，淘宝会通知码商系统订单已经退款成功。码商系统在收到退款成功通知后需要给淘宝收到通知的反馈，
        /// 同时可以根据自己的业务需要做退款成功后的处理，例如：作废码等。注意，码商对接此接口需保持幂等，若已处理过退款成功请求，请直接返回成功。
        /// </summary>
        /// <param name="dto"></param>
        public Result HandleRefundBusiness(RefundNotificationDto dto)
        {
            var sku = AliBusinessHelper.GetParkIdAndTicketTypeId(dto.OuterIdSKU);
            //告诉票务系统作废码
            var cancelResult = AliBusinessHelper.OrderCancel(dto.OuterId, dto.OrganizerNick, sku[0]);

            //作废操作成功
            if (cancelResult?.ResultStatus == ResultState.OrderCancelSuccess)
            {
                //本地添加退款记录
                _tmallOrderRefundrRepository.InsertAndGetId(new TmallOrderRefund()
                {
                    OuterId = dto.OuterId,
                    RefundFee = string.IsNullOrWhiteSpace(dto.RefundFee) ? 0 : int.Parse(dto.RefundFee) / 100, /*decimal.Parse(model.refundFee)*/
                    ItemId = dto.ItemId,
                    ItemTitle = dto.ItemTitle,
                    CancelNum = dto.CancelNum,
                    IdCard = dto.IdCard,
                    OrganizerNick = dto.OrganizerNick,
                    Amount = dto.Amount,
                    ValidEnd = dto.ValidEnd,
                    ValidStart = dto.ValidStart,
                    Status = AliBusinessNotificationMethod.RefundSuccess
                });

                _tmallOrderDetailRepository.Update(dto.OuterId,
                    m => Task.FromResult(m.Status = OrderStatus.TRADE_CLOSED.ToString()));
                Logger.Info("退款业务中订单：" + dto.OuterId + "操作退款成功");
                return Result.Ok();
            }
            else if(cancelResult?.ResultStatus == ResultState.OrderAlreadyCancel)
            {
                Logger.Info("退款业务中订单：" + dto.OuterId + "已经取消");
                return Result.Ok();
            }
            //else if (cancelResult?.ResultStatus == ResultState.InvalidToken)
            //{
            //    AliBusinessHelper.TokenEndTime = DateTime.Now.AddHours(-1);
            //}
            Logger.Error("订单号："+ dto.OuterId + "退款失败，票务系统返回错误原因:" + cancelResult?.ToJsonString());
            return Result.FromError("操作退款失败，票务系统返回错误原因:" + cancelResult?.Message);
        }

        #endregion
    }
}
