using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Json;
using Castle.Core.Logging;
using ThemePark.AliPartner.Constants;
using ThemePark.AliPartner.Model;
using ThemePark.AliPartner.TopSdk;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Enum;
using ThemePark.Core.AliPartner;
using System.Threading;
using ThemePark.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness.Helper
{
    /// <summary>
    /// 核销订单作业
    /// </summary>
    public class ConsumeTicketJob : ITransientDependency
    {
        private readonly ILogger _logger;

        /// <summary>
        /// cotr
        /// </summary>
        public ConsumeTicketJob()
        {
            _logger = IocManager.Instance.Resolve<ILoggerFactory>().Create(typeof(ConsumeTicketJob));
        }


        /// <summary>
        /// 执行作业
        /// </summary>
        public void Execute(int parkId)
        {
            while (true)
            {
                if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour <= 23)
                {
                    Stopwatch watchAll = new Stopwatch();
                    watchAll.Start();
                    // 对本地数据库进行查询需要核销的订单
                    var date = DateTime.Today.Date;
                    var dbContext = IocManager.Instance.Resolve<ThemeParkDbContext>();
                    var consumList = dbContext.TmallOrderDetails
                        .Where(m => m.Status == OrderStatus.WAIT_BUYER_CONFIRM_GOODS.ToString() && m.ParkId == parkId)
                        .Join(
                            dbContext.TmallOrders.Where(m =>
                                DbFunctions.DiffDays(m.ValidStart, date) >= 0 &&
                                DbFunctions.DiffDays(m.ValidEnd, date) <= 0),
                            orderDetail => orderDetail.MainOrderId, order => order.Id,
                            (orderDetail, order) => new OrderConsumeArgs
                            {
                                ItemTitle = orderDetail.ItemTitle,
                                ParkId = orderDetail.ParkId.ToString(),
                                OrganizerNick = order.OrganizerNick,
                                Token = orderDetail.Token,
                                Ticketcode = orderDetail.TicketCode,
                                OrderId = orderDetail.Id,
                                MainOrderId = order.Id,
                                FtOrderId = orderDetail.FtOrderId,
                                OrderCreationTime = order.CreationTime
                            });

                    //如果该公园已经没有需要核销的数据，将休眠10分钟
                    if (!consumList.Any())
                    {
                        _logger.Info("公园编号" + parkId + "，已没有需要核销的订单，将休眠10分钟再检测");
                        Thread.Sleep(1000 * 60 * 10);
                        continue;
                    }

                    int oneRoundTotal = consumList.Count();


                    //如果该公园需要核销的数据小于5单，将休眠5分钟后再核销，避免日志数量膨胀
                    if (oneRoundTotal <= 5)
                    {
                        _logger.Info("公园编号" + parkId + "，数量为" + oneRoundTotal + "小于等于5，将休眠5分钟再进行核销检测");
                        Thread.Sleep(1000 * 60 * 5);
                    }

                    var successCount = 0;
                    foreach (var arg in consumList)
                    {
                        Thread.Sleep(50);
                        Stopwatch watch = Stopwatch.StartNew();
                        watch.Start();
                        try
                        {
                            //票务系统订单状态查询
                            var detail = AliBusinessHelper.GetOrderDetailFromFt(arg.OrganizerNick, arg.ParkId, arg.OrderId);

                            if (detail?.ResultStatus == ResultState.Ok)
                            {
                                //检测是否已取票
                                if (detail.Data.Any(item => item.Planstate.Equals("2")))
                                {
                                    //获取核销份数
                                    var consumeCount = detail.Data.Where(o => o.Planstate.Equals("2")).Sum(temp => temp.Number);

                                    //调用淘宝SDK进行核销
                                    var result = TopSdkHelper.Consume(arg.Ticketcode, consumeCount, arg.OrderId, arg.OrderId,
                                       arg.Token, arg.OrderCreationTime);

                                    if (result.Success)
                                    {
                                        successCount++;
                                        var consume = dbContext.TmallAuthSends.FirstOrDefault(m => m.OuterId == arg.OrderId);
                                        if (consume != null)
                                        {
                                            //重复核销的问题
                                            _logger.Info("公园编号" + parkId + "，" + arg.OrganizerNick + "订单：" + arg.OrderId + result.Message + "|费时：" + watch.ElapsedMilliseconds + "毫秒");
                                            continue;
                                        }

                                        var record = new TmallAuthSend
                                        {
                                            OuterId = arg.OrderId,
                                            Status = AliBusinessNotificationMethod.ConsumeSuccess,
                                            ItemTitle = arg.ItemTitle,
                                            OrganizerNick = arg.OrganizerNick
                                        };
                                        //添加本地核销记录和更改本地订单状态
                                        dbContext.TmallAuthSends.Add(record);
                                        //tmallOrderRepository.Update(arg.MainOrderId,
                                        //    m => Task.FromResult(m.Status = OrderStatus.TRADE_FINISHED.ToString()));
                                        //tmallOrderDetailRepository.Update(arg.OrderId,
                                        //    m => Task.FromResult(m.Status = OrderStatus.TRADE_FINISHED.ToString()));
                                        var tmallOrder = dbContext.TmallOrders.Find(arg.MainOrderId);
                                        tmallOrder.Status = OrderStatus.TRADE_FINISHED.ToString();
                                        dbContext.TmallOrders.AddOrUpdate(tmallOrder);
                                        var tmallOrderDetail = dbContext.TmallOrderDetails.Find(arg.MainOrderId);
                                        tmallOrderDetail.Status = OrderStatus.TRADE_FINISHED.ToString();
                                        dbContext.TmallOrderDetails.AddOrUpdate(tmallOrderDetail);
                                        dbContext.SaveChanges();
                                        watch.Stop();
                                        _logger.Info("公园编号" + parkId + "，" + arg.OrganizerNick + "订单：" + arg.OrderId + result.Message + "|费时：" + watch.ElapsedMilliseconds + "毫秒");
                                        continue;
                                    }
                                    watch.Stop();
                                    _logger.Error("公园编号" + parkId + "，" + arg.OrganizerNick + "订单：" + arg.OrderId + "核销失败，原因为：\r\n" + result.Message + "|费时：" + watch.ElapsedMilliseconds + "毫秒");
                                }
                            }
                            else if (detail?.ResultStatus == ResultState.InvalidToken)
                            {
                                switch (arg.OrganizerNick)
                                {
                                    //天猫
                                    case AliApplicationSetting.FangteTravel:
                                        {
                                            AliBusinessHelper.SetFangteTravelTokenEndTime(DateTime.Now.AddHours(-1));
                                            break;
                                        }
                                    //淘宝
                                    case AliApplicationSetting.FangteThemePark:
                                        {
                                            AliBusinessHelper.SetFangteThemeParkTokenEndTime(DateTime.Now.AddHours(-1));
                                            break;
                                        }
                                }
                                watch.Stop();
                                _logger.Error("公园编号" + parkId + "，" + arg.OrganizerNick + "订单：" + arg.OrderId + "核销失败，原因为：\r\n" + "token失效" + "|费时：" + watch.ElapsedMilliseconds + "毫秒");
                            }
                            else
                            {
                                watch.Stop();
                                _logger.Error("公园编号" + parkId + "，从票务系统查询" + arg.OrganizerNick + "订单：" + arg.OrderId + "失败，原因为：\r\n" + detail.ToJsonString() + "|费时：" + watch.ElapsedMilliseconds + "毫秒");
                            }
                        }
                        catch (Exception e)
                        {
                            watch.Stop();
                            _logger.Error("公园编号" + parkId + "，核销" + arg.OrganizerNick + "订单：" + arg.OrderId + "发生异常，原因为：" + e
                                                                                  + "|费时：" + watch.ElapsedMilliseconds
                                                                                  + "毫秒");
                        }
                    }
                    watchAll.Stop();
                    _logger.Info("公园编号" + parkId + "，本轮核销订单数量：" + oneRoundTotal + "，成功核销" + successCount
                                                                           + "|费时：" + watchAll.ElapsedMilliseconds
                                                                           + "毫秒");
                }
                Thread.Sleep(1000);
            }
        }
    }
}
