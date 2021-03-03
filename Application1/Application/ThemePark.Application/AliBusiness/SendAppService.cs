using System;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using ThemePark.AliPartner.Constants;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Enum;
using ThemePark.Application.AliBusiness.Helper;
using ThemePark.Application.AliBusiness.Interfaces;
using ThemePark.Common;
using ThemePark.Core.AliPartner;

namespace ThemePark.Application.AliBusiness
{
    /// <summary>
    /// 发码应用服务
    /// </summary>
    public class SendAppService : ThemeParkAppServiceBase, ISendAppService
    {
        #region Fields
        private readonly IDatabase _redisDatabase;
        #endregion

        #region Cotr
        /// <summary>
        /// cotr
        /// </summary>
        public SendAppService()
        {
            _redisDatabase = IocManager.Instance.Resolve<IAbpRedisCacheDatabaseProvider>().GetDatabase();
        }

        /// <summary>
        /// cotr
        /// </summary>
        //public SendAppService()
        //{
        //}

        #endregion

        #region Public Methods

        /// <summary>
        /// 卖家在入驻淘宝电子凭证平台后可以发布电子凭证商品，用户在淘宝购买电子凭证商品后，淘宝会通知对应的码商进行发码操作，码商收到通知后需要同步返回响应结果给淘宝平台。 
        /// 码商系统在收到淘宝的发码通知后，需要进行发码的一系列操作，包括但不限于给消费者手机发送相应的凭证码,回调淘宝平台提供的发码回调(异步)接口等
        /// 
        /// !!!!!!!!!!发码是一笔子订单发一个请求过来，所以这个请求传过来的数量就是这笔子订单的数量
        /// </summary>
        /// <returns></returns>
        [DisableAuditing]
        public void HandleSendBusiness(SendAndResendNotificationDto dto)
        {
            //Logger.Info("发码流程开始");
            TmallOrder tmalllOrder = AliBusinessHelper.TradeTransfer(dto);

            TmallOrderDetail tmallOrderDetail = new TmallOrderDetail
            {
                Id = dto.OuterId,
                Num = dto.Amount,
                //有可能没传身份证过来
                IdCard = dto.IdCard ?? string.Empty,
                ItemId = dto.ItemId,
                ItemTitle = dto.ItemTitle,
                MainOrderId = dto.MainOrderId,
                //这个没从发码请求过来
                Payment = Convert.ToDecimal(tmalllOrder?.Payment),
                //这个没从发码请求过来
                Price = tmalllOrder?.Payment / dto.Amount,
                Status = tmalllOrder?.Status,
                Token = dto.Token,
                OuterIdSKU = dto.OuterIdSKU,
                ParkId = int.Parse(AliBusinessHelper.GetParkIdAndTicketTypeId(dto.OuterIdSKU)[0])
            };

            //缓存数据
            CacheData(tmalllOrder, tmallOrderDetail);
        }

        #endregion

        #region Private Methods

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

            //FtOrderDetailDto属性名需小写，不然旧系统识别不了
            var orderDetail = new FtOrderDetailDto
            {
                idnum = string.IsNullOrWhiteSpace(tmallOrderDetail.IdCard) ? string.Empty : tmallOrderDetail.IdCard,
                name = string.IsNullOrWhiteSpace(tmallOrder.Name) ? "飞猪用户" : tmallOrder.Name,
                number = tmallOrderDetail.Num,
                tickettypeid = outerIdSku[1],
                saleprice = tmallOrderDetail.Price
            };
            //var orderDetailJson = JsonConvert.SerializeObject(detail);
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

            Work.Retry(() => _redisDatabase.ListRightPush(AliBusinessNotificationMethod.Send, obj), exception => { },
                exception => { Logger.Error("[send]订单：" + tmallOrder.Id + "插入redis缓存队列失败，原因为：\r\n" + exception.Message); }, 3);

        }

        #endregion
    }
}
