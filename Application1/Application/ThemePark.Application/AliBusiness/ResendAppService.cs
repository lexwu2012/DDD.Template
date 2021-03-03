using System;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Runtime.Caching.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using ThemePark.AliPartner.Constants;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Helper;
using ThemePark.Application.AliBusiness.Interfaces;
using ThemePark.Common;
using ThemePark.Core.AliPartner;

namespace ThemePark.Application.AliBusiness
{
    /// <summary>
    /// 重发码应用服务
    /// </summary>
    public class ResendAppService : ThemeParkAppServiceBase, IResendAppService
    {
        #region Fields

        private readonly IDatabase _redisDatabase;

        #endregion

        #region Cotr

        /// <summary>
        /// cotr
        /// </summary>
        public ResendAppService()
        {
            _redisDatabase = IocManager.Instance.Resolve<IAbpRedisCacheDatabaseProvider>().GetDatabase();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 在没有收到凭证码等情况下，买家或者卖家都可以在淘宝平台上发起重新发码的操作，淘宝会通知码商系统进行重新发码操作，码商系统收到通知后，需要给淘宝收到通知的反馈。
        ///  码商系统在收到淘宝的发码通知后，需要进行重新发码的一些操作，包括但不限于给用户手机重新发送相应的凭证码（需要根据用户通知类型决定是否给用户手机发码），
        /// 回调淘宝平台提供的重新发码回调接口等。
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public void HandleResendBusiness(SendAndResendNotificationDto dto)
        {
            //从本地数据库获取码
            var tmallOrderDetailRepository = IocManager.Instance.Resolve<IRepository<TmallOrderDetail, string>>();
            var detail = tmallOrderDetailRepository.FirstOrDefault(dto.OuterId);

            if (!string.IsNullOrEmpty(detail?.TicketCode))
            {
                //重发码
                var result = AliBusinessHelper.ResendECode(detail.TicketCode, dto.IdCard, dto.OuterId, Convert.ToInt32(dto.Amount), detail.Token, string.Empty);
                if (!string.IsNullOrWhiteSpace(result?.RetCode) && (result.RetCode.Equals(NotificationResult.SuccessAll) || result.RetCode.Equals(NotificationResult.AlreaySend)))
                    Logger.Info("订单号：" + dto.OuterId + "重发码成功");
                else
                    Logger.Error(result?.ToJsonString());
            }
            //else
            //{
            //    //Logger.Info("发码流程开始");
            //    TmallOrder tmalllOrder = AliBusinessHelper.TradeTransfer(dto);

            //    TmallOrderDetail tmallOrderDetail = new TmallOrderDetail
            //    {
            //        Id = dto.OuterId,
            //        Num = dto.Amount,
            //        //有可能没传身份证过来
            //        IdCard = dto.IdCard ?? string.Empty,
            //        ItemId = dto.ItemId,
            //        ItemTitle = dto.ItemTitle,
            //        MainOrderId = dto.MainOrderId,
            //        //这个没从发码请求过来
            //        Payment = Convert.ToDecimal(tmalllOrder?.Payment),
            //        //这个没从发码请求过来
            //        Price = tmalllOrder?.Payment / dto.Amount,
            //        Status = tmalllOrder?.Status,
            //        Token = dto.Token,
            //        OuterIdSKU = dto.OuterIdSKU,
            //        ParkId = int.Parse(AliBusinessHelper.GetParkIdAndTicketTypeId(dto.OuterIdSKU)[0])
            //    };

            //    //缓存数据
            //    CacheData(tmalllOrder, tmallOrderDetail);
            //}
        }

        /// <summary>
        /// 缓存阿里请求过来的订单数据，然后通过工作管理者添加作业获取取票码
        /// </summary>
        /// <param name="tmallOrder"></param>
        /// <param name="tmallOrderDetail"></param>
        /// <returns></returns>
        private void CacheData(TmallOrder tmallOrder, TmallOrderDetail tmallOrderDetail)
        {
            var outerIdSku = AliBusinessHelper.GetParkIdAndTicketTypeId(tmallOrderDetail.OuterIdSKU);
            var parkId = outerIdSku[0];

            var orderDetail = new FtOrderDetailDto
            {
                idnum = string.IsNullOrWhiteSpace(tmallOrderDetail.IdCard) ? string.Empty : tmallOrderDetail.IdCard,
                name = string.IsNullOrWhiteSpace(tmallOrder.Name) ? "飞猪用户" : tmallOrder.Name,
                number = tmallOrderDetail.Num,
                tickettypeid = outerIdSku[1],
                saleprice = tmallOrderDetail.Price
            };

            var order = new OrderSendToFtDataArgs
            {
                PlaceFtOrderEntity = new PlaceFtOrderEntity
                {
                    Outerorderid = tmallOrderDetail.Id,
                    Parkid = parkId,
                    Paydate = DateTime.Now.ToString(),
                    Paymodename = "支付宝",
                    Phone = tmallOrder.Mobile,
                    Plandate = tmallOrder.ValidStart.Value.ToShortDateString(),
                    TicketList = orderDetail,
                    Totalmoney = tmallOrderDetail.Payment.Value.ToString(),
                    Tradeno = tmallOrderDetail.Id,
                    Enddate = tmallOrder.ValidEnd.Value.ToString()
                },
                TmallOrder = tmallOrder,
                TmallOrderDetail = tmallOrderDetail
            };

            var obj = JsonConvert.SerializeObject(order);

            Work.Retry(() => _redisDatabase.ListRightPush(AliBusinessNotificationMethod.ReSend, obj), exception => { },
                exception => { Logger.Error("[resend]订单号：" + tmallOrder.Id + "插入redis缓存队列失败，原因为：\r\n" + exception.Message); }, 3);

        }

        #endregion
    }
}
