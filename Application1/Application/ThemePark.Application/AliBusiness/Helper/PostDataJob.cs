using System;
using System.Diagnostics;
using System.Threading;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Logging;
using Abp.Runtime.Caching.Redis;
using Castle.Core.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using ThemePark.AliPartner.Constants;
using ThemePark.AliPartner.Model;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Enum;
using ThemePark.Common;
using ThemePark.Core.AliPartner;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness.Helper
{
    /// <summary>
    /// 提交数据到票务系统作业
    /// </summary>
    public class PostDataJob : ITransientDependency
    {
        #region Fields
        private readonly IDatabase _redisDatabase;
        private readonly ILogger _logger;
        #endregion

        /// <summary>
        /// cotr
        /// </summary>
        public PostDataJob()
        {
            _redisDatabase = IocManager.Instance.Resolve<IAbpRedisCacheDatabaseProvider>().GetDatabase();
            _logger = IocManager.Instance.Resolve<ILoggerFactory>().Create(typeof(PostDataJob));
        }

        /// <summary>
        /// 下单且发码
        /// </summary>
        public void SendData(string method)
        {
            while (true)
            {
                Thread.Sleep(10);
                var arg = GetPostDataJobArgsFromCache(method);
                if (arg == null)
                {
                    Thread.Sleep(500);
                    continue;
                }
                Stopwatch watch = new Stopwatch();
                watch.Start();

                var orderTicketUrl = AliAppSetingsModel.TicketServerUrl + AliAppSetingsModel.OrderTicketUrl;
                try
                {
                    var param = CombineParams(arg, method);
                    if (string.IsNullOrWhiteSpace(param))
                    {
                        Thread.Sleep(500);
                        continue;
                    }

                    //下单
                    var result = HttpHelper.HttpPost(orderTicketUrl, param.Substring(0, param.Length - 1));

                    var resultObj = JsonConvert.DeserializeObject<ReturnResultEntity<OrderTicketReturnDto>>(result);

                    //下单成功（104为订单已存在）
                    if (resultObj?.Data != null && (resultObj.ResultStatus == ResultState.OrderSuccess || resultObj.ResultStatus == ResultState.OrderAlreadyExist))
                    {
                        //给阿里平台发取票码
                        var sendResult = AliBusinessHelper.SendECode(resultObj.Data.Ticketcode,
                                               arg.PlaceFtOrderEntity.TicketList.idnum,
                                               arg.PlaceFtOrderEntity.Outerorderid, arg.PlaceFtOrderEntity.TicketList.number,
                                               arg.TmallOrderDetail.Token, string.Empty);

                        if (!string.IsNullOrWhiteSpace(sendResult?.RetCode) && (sendResult.RetCode.Equals(NotificationResult.SuccessAll) || sendResult.RetCode.Equals(NotificationResult.AlreaySend)))
                        {
                            watch.Stop();
                            _logger.Info("[" + method + "]" + arg.TmallOrder.OrganizerNick + "订单号："
                                + arg.PlaceFtOrderEntity.Outerorderid
                                + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU
                                + "，结果：" + sendResult.RetMsg
                                + "，费时：" + watch.ElapsedMilliseconds + "毫秒");

                            DataOperation(arg, resultObj.Data.Ticketcode, resultObj.Data.Orderid);
                            continue;
                        }
                        else
                        {
                            watch.Stop();
                            _logger.Error("[" + method + "]" + arg.TmallOrder.OrganizerNick + "订单号："
                                + arg.PlaceFtOrderEntity.Outerorderid
                                 + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU
                                + "，发码失败，原因为：\r\n" + sendResult?.ToJsonString()
                                + "\r\n|费时：" + watch.ElapsedMilliseconds
                                + "毫秒");

                            //todo: 由于业务变更（可售卖当天票），造成如果当天发码不成功的话，天猫会默认过两个小时退款给游客，而中心的订单是正常的，如果这时游客刷身份证的话是可以入园
                            if (sendResult != null && sendResult.IsError && sendResult.SubErrCode.Equals(NotificationResult.InvalidSession))
                            {
                                //取消订单
                                var cancelResult = AliBusinessHelper.OrderCancel(arg.TmallOrderDetail.Id, arg.TmallOrder.OrganizerNick, arg.TmallOrderDetail.ParkId.ToString());

                                //作废操作成功
                                if (cancelResult?.ResultStatus == ResultState.OrderCancelSuccess || cancelResult?.ResultStatus == ResultState.OrderAlreadyCancel)
                                {
                                    _logger.Info("[" + method + "]" + arg.TmallOrder.OrganizerNick + "订单号："
                                        + arg.PlaceFtOrderEntity.Outerorderid
                                        + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU
                                        + "发码失败，取消中心订单成功，"
                                        + "，费时：" + watch.ElapsedMilliseconds + "毫秒");
                                }
                                else
                                {
                                    _logger.Error("[" + method + "]" + arg.TmallOrder.OrganizerNick + "订单号："
                                        + arg.PlaceFtOrderEntity.Outerorderid
                                        + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU
                                        + "发码失败，取消中心订单失败，原因为：" + cancelResult?.ResultStatus.DisplayName()
                                        + "，费时：" + watch.ElapsedMilliseconds + "毫秒");
                                }
                                continue;
                            }
                        }

                        ////发码成功后才进行数据修改
                        //if (sendResult.Success)
                        //{
                        //    watch.Stop();
                        //    _logger.Info("[" + method + "]"+ arg.TmallOrder.OrganizerNick + "订单号："
                        //        + arg.PlaceFtOrderEntity.Outerorderid
                        //        + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU
                        //        + "，结果：" + sendResult.Message
                        //        + "，费时：" + watch.ElapsedMilliseconds + "毫秒");

                        //    DataOperation(arg, resultObj.Data.Ticketcode, resultObj.Data.Orderid);
                        //    continue;
                        //}
                        //else
                        //{
                        //    //todo: 由于业务变更（可售卖当天票），这样会造成如果当天发码不成功的话，天猫会默认过两个小时退款给游客，而中心的订单是正常的，如果这时游客刷身份证可以入园
                        //    watch.Stop();
                        //    _logger.Error("[" + method + "]" + arg.TmallOrder.OrganizerNick + "订单号："
                        //        + arg.PlaceFtOrderEntity.Outerorderid
                        //         + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU
                        //        + "，发码失败，原因为：\r\n" + sendResult.Message
                        //        + "|费时：" + watch.ElapsedMilliseconds
                        //        + "毫秒");
                        //}
                    }
                    else
                    {
                        //下单不成功
                        //TODO：什么类型的错误需要重新入队？                        
                        watch.Stop();
                        _logger.Error("[" + method + "]" + arg.TmallOrder.OrganizerNick + "订单号：" + arg.PlaceFtOrderEntity.Outerorderid + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU +
                                      "，向方特系统下单失败，原因为：\r\n" + resultObj?.Message + "|费时：" + watch.ElapsedMilliseconds +
                                      "毫秒");
                        //由于客服原因上错票类，没及时更新票类日期，身份证超出预定数量等业务错误，不需要把该订单重新入队，不然会一直写日志
                        if (resultObj?.Data != null && (resultObj.ResultStatus == ResultState.OrderPlanDataError
                            || resultObj.ResultStatus == ResultState.OrderTicketTypeIdError
                            || resultObj.ResultStatus == ResultState.OrderIdnumUseLimit
                            || resultObj.ResultStatus == ResultState.IdnumError))
                        {
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    //TODO：如果下单超时抛出异常，需要重新把订单数据入队
                    watch.Stop();
                    _logger.Error("[" + method + "]" + arg.TmallOrder.OrganizerNick + "订单号：" + arg.PlaceFtOrderEntity.Outerorderid + "，预订公园/票类：" + arg.TmallOrderDetail.OuterIdSKU + "发生异常，原因为：\r\n"
                        + e.InnerException + "|费时：" + watch.ElapsedMilliseconds + "毫秒");
                }
                Work.Retry(() => _redisDatabase.ListRightPush(method, JsonConvert.SerializeObject(arg)), exception => { },
                    exception => { _logger.Error("插入redis缓存队列失败，原因为：\r\n" + exception.Message); }, 3);
            }
        }



        public void Test(string method)
        {
            while (true)
            {
                Thread.Sleep(10);
                var arg = GetPostDataJobArgsFromCache(method);
                if (arg == null)
                {
                    Thread.Sleep(500);
                    continue;
                }
                Stopwatch watch = new Stopwatch();
                watch.Start();

                var orderTicketUrl = AliAppSetingsModel.TicketServerUrl + AliAppSetingsModel.OrderTicketUrl;
                try
                {
                    var param = CombineParams(arg, method);
                    if (string.IsNullOrWhiteSpace(param))
                    {
                        Thread.Sleep(500);
                        continue;
                    }

                    var result = HttpHelper.HttpPost(orderTicketUrl, param.Substring(0, param.Length - 1));

                    var resultObj = JsonConvert.DeserializeObject<ReturnResultEntity<OrderTicketReturnDto>>(result);

                    //下单成功（104为订单已存在）
                    if (resultObj?.Data != null && (resultObj.ResultStatus == ResultState.OrderSuccess || resultObj.ResultStatus == ResultState.OrderAlreadyExist))
                    {
                        //给阿里平台发码
                        var sendResult = Result.Ok();

                        //发码成功后才进行数据修改
                        if (sendResult.Success)
                        {
                            watch.Stop();
                            _logger.Info("[" + method + "]订单号：" + arg.PlaceFtOrderEntity.Outerorderid + "下单且发码成功，费时：" + watch.ElapsedMilliseconds + "毫秒");

                            DataOperation(arg, resultObj.Data.Ticketcode, resultObj.Data.Orderid);
                            continue;
                        }
                        else
                        {
                            watch.Stop();
                            _logger.Error("[" + method + "]订单号：" + arg.PlaceFtOrderEntity.Outerorderid +
                                          "发码失败，原因为：\r\n" + sendResult.Message + "|费时：" + watch.ElapsedMilliseconds +
                                          "毫秒");
                        }
                    }
                    else
                    {
                        //下单不成功
                        //TODO：什么类型的错误需要重新入队？
                        watch.Stop();
                        _logger.Error("[" + method + "]订单号：" + arg.PlaceFtOrderEntity.Outerorderid +
                                      "向方特系统下单失败，原因为：\r\n" + resultObj?.Message + "|费时：" + watch.ElapsedMilliseconds +
                                      "毫秒");
                        //if (resultObj != null && resultObj.ResultStatus == ResultState.InvalidToken)
                        //{
                        //    AliBusinessHelper.TokenEndTime = DateTime.Now.AddHours(-1);
                        //}
                    }
                }
                catch (Exception e)
                {
                    //TODO：如果下单超时抛出异常，需要重新把订单数据入队
                    watch.Stop();
                    _logger.Error("[" + method + "]订单号：" + arg.PlaceFtOrderEntity.Outerorderid + "方法异常，原因为：\r\n" + e.InnerException + "|费时：" + watch.ElapsedMilliseconds + "毫秒");
                }
                Work.Retry(() => _redisDatabase.ListRightPush(method, JsonConvert.SerializeObject(arg)), exception => { },
                    exception => { _logger.Error("插入redis缓存队列失败，原因为：\r\n" + exception.Message); }, 3);
            }
        }

        #region Private Methods

        /// <summary>
        /// 获取下单参数
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private OrderSendToFtDataArgs GetPostDataJobArgsFromCache(string method)
        {
            var watcher = Stopwatch.StartNew();
            watcher.Start();
            try
            {
                var value = _redisDatabase.ListLeftPop(method);

                if (string.IsNullOrWhiteSpace(value))
                {
                    watcher.Stop();
                    return null;
                }

                var desData = JsonConvert.DeserializeObject<OrderSendToFtDataArgs>(value);
                watcher.Stop();
                return desData;

            }
            catch (Exception e)
            {
                watcher.Stop();
                //LogHelper.LogException(_logger,e);
                _logger.Error("[" + method + "]读取redis缓存异常，费时：" + watcher.ElapsedMilliseconds + "，异常为：\r\n" + e.Message + "\r\n" + e.InnerException);
            }

            watcher.Stop();
            return null;
        }

        /// <summary>
        /// 持久化到数据库
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="ticketCode"></param>
        /// <param name="orderId"></param>
        private void DataOperation(OrderSendToFtDataArgs arg, string ticketCode, string orderId)
        {
            //发货成功后才进行数据操作
            arg.TmallOrder.Status = TradeStatus.WAIT_BUYER_CONFIRM_GOODS.ToString();
            arg.TmallOrderDetail.Status = OrderStatus.WAIT_BUYER_CONFIRM_GOODS.ToString();

            //把码和票务系统的订单号存储在本地
            arg.TmallOrderDetail.TicketCode = ticketCode;
            arg.TmallOrderDetail.FtOrderId = orderId;

            //本地保存订单数据
            var tmallOrderRepository = IocManager.Instance.Resolve<IRepository<TmallOrder, string>>();
            var tmallOrderDetailRepository = IocManager.Instance.Resolve<IRepository<TmallOrderDetail, string>>();

            tmallOrderRepository.InsertOrUpdateAndGetId(arg.TmallOrder);

            tmallOrderDetailRepository.InsertOrUpdateAndGetId(arg.TmallOrderDetail);
        }

        /// <summary>
        /// 组装OTA接口参数
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private string CombineParams(OrderSendToFtDataArgs arg, string method)
        {
            //var tokenEntity = AliBusinessHelper.GetFtToken(arg.TmallOrder.OrganizerNick);

            var tokenEntity = new FtTokenEntity();
            switch (arg.TmallOrder.OrganizerNick)
            {
                //天猫
                case AliApplicationSetting.FangteTravel:
                    tokenEntity = AliBusinessHelper.GetFtToken4Tmall();
                    break;
                //淘宝
                case AliApplicationSetting.FangteThemePark:
                    tokenEntity = AliBusinessHelper.GetFtToken4Taobao();
                    break;
            }

            if (tokenEntity?.ResultStatus == 0)
            {
                arg.PlaceFtOrderEntity.Token = tokenEntity.Data.Token;
                var orderjson = JsonConvert.SerializeObject(arg.PlaceFtOrderEntity.TicketList);
                var param = string.Empty;
                param += AliBusinessHelper.TakeParam("ticketList", "[" + orderjson + "]");
                param += AliBusinessHelper.TakeParam("token", arg.PlaceFtOrderEntity.Token);
                param += AliBusinessHelper.TakeParam("parkid", arg.TmallOrderDetail.ParkId.ToString());
                if (arg.TmallOrder.ValidStart != null)
                    param += AliBusinessHelper.TakeParam("plandate", arg.TmallOrder.ValidStart.Value.ToShortDateString());
                if (arg.TmallOrder.ValidEnd != null)
                    param += AliBusinessHelper.TakeParam("enddate", arg.TmallOrder.ValidEnd.Value.ToShortDateString());

                param += AliBusinessHelper.TakeParam("phone", arg.TmallOrder.Mobile);
                param += AliBusinessHelper.TakeParam("totalmoney", arg.TmallOrderDetail.Payment.Value.ToString());
                param += AliBusinessHelper.TakeParam("paymodename", "支付宝");
                param += AliBusinessHelper.TakeParam("outorderid", arg.TmallOrderDetail.Id);
                param += AliBusinessHelper.TakeParam("paydate", DateTime.Now.ToString());
                param += AliBusinessHelper.TakeParam("tradeno", arg.TmallOrderDetail.Id);

                return param;
            }
            //AliBusinessHelper.TokenEndTime = DateTime.Now.AddHours(-1);
            //获取token失败时重新缓存数据
            _redisDatabase.ListRightPush(method, JsonConvert.SerializeObject(arg));
            _logger.Error("发码流程中订单为：" + arg.TmallOrder.Id + "请求票务会话令牌出错，错误为：\r\n" + tokenEntity?.Message);
            return string.Empty;
        }

        #endregion
    }
}


