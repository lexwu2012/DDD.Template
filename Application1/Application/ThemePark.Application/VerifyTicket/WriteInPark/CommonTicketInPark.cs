using System;
using System.Data.Entity.Core;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Castle.Core.Logging;
using EntityFramework.DynamicFilters;
using Newtonsoft.Json;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.DataSync;
using ThemePark.Core.InPark;
using ThemePark.Core.ParkSale;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.VerifyTicket.WriteInPark
{
    internal class CommonTicketInPark : WriteInParkBase
    {
        public override WriteInParkJobArgs GetJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            var inParkPerson = ticketCheckData.AllowPersons - noPast;
            ticketCheckData.InPersons += inParkPerson;

            //本公园允许入园人数
            var perPark = ticketCheckData.GetInParkRull().InParkTimesPerPark * ticketCheckData.GetPersons();
            
            if (perPark == ticketCheckData.InPersons)
            {
                ticketCheckData.CheckState = CheckState.Used;
            }
            else
            {
                ticketCheckData.CheckState = CheckState.Idle;
            }

            //总共还剩下多少次数
            var temp = perPark - ticketCheckData.InPersons;
            
            //剩余次数小于单票次人数（取小的）
            if (temp < ticketCheckData.GetPersons())
            {
                ticketCheckData.CommonTicketDto.Persons = temp.Value;
            }
            else
            {
                ticketCheckData.CommonTicketDto.Persons = ticketCheckData.GetPersons().Value;
            }

            //ticketCheckData.CommonTicketDto.Persons = ticketCheckData.GetPersons().Value - ticketCheckData.InPersons;

            ticketCheckData.AllowPersons = ticketCheckData.CommonTicketDto.Persons;

            // 更新表数据
            // 构造写入园记录作业的参数
            var jobArgs = new WriteInParkJobArgs
            {
                Terminal = terminal,
                VerifyType = ticketCheckData.VerifyType,
                VerifyCode = id,

                //获取本地公园编号
                ParkId = LocalParkId,
                TicketSaleStatus = ticketCheckData.CheckState == CheckState.Used ? (int)TicketSaleStatus.InPark : (int)TicketSaleStatus.Valid,
                TableName = ticketCheckData.TableName,
                EntityType = ticketCheckData.EntityType,
                InParkCount = ticketCheckData.InPersons,
                Persons = inParkPerson,
                TicketClassId = ticketCheckData.GetTicketClassCacheItem().TicketClassId,
                InParkIdFilter = ticketCheckData.GetTicketClassCacheItem().CanInParkIds,
                Remark = remark
            };

            return jobArgs;
        }

        public override void UpdateDB(WriteInParkJobArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.TableName))
                return;

            string suborder = string.Empty;

            if (args.Persons > 0)
            {
                // 入园信息：日期 公园ID 人数，如: 2017-3-15 41 3, 
                string inParkText = DateTime.Today.ToString("yyyy-MM-dd")
                    + " " + args.ParkId + " " + args.Persons + ",";

                //确保释放对象，防止内存泄漏
                using (var dispose = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
                {
                    var context = dispose.Object;
                    var set = context.Set(args.EntityType);

                    context.DisableFilter(DataFilters.ParkPermission);
                    dynamic ticket = set.Find(args.VerifyCode);
                    if (ticket == null)
                    {
                        throw new ObjectNotFoundException($"{args.VerifyCode} not found in {args.TableName}");
                    }

                    var toTicket = ticket as TOTicket;
                    if (toTicket?.InparkCounts == 0)
                    {
                        suborder = toTicket.TOVoucher?.TOBodyId ??
                                   context.TOVouchers.Where(o => o.Id == toTicket.TOVoucherId).Select(o => o.TOBodyId).Single();
                    }

                    ticket.InparkCounts = args.InParkCount;
                    ticket.TicketSaleStatus = (TicketSaleStatus)args.TicketSaleStatus;
                    ticket.Remark += inParkText;

                    context.SaveChanges();
                }
            }

            // 如果是OTA的订单票，同步中心订单数据
            if (args.TableName == typeof(TOTicket).Name && !string.IsNullOrEmpty(suborder))
            {
                var data = new OrderConsumeDto() { SubOrderid = suborder };
                var tobodyDomainAppservice = IocManager.Instance.Resolve<ITOBodyDomainService>();
                tobodyDomainAppservice.ConsumedTOBody(data.SubOrderid);
                var sync = IocManager.Instance.Resolve<IDataSyncManager>();
                sync.UploadDataToTargetPark(ThemeParkConsts.CenterParkId, new DataSyncInput() { SyncType = DataSyncType.OrderConsume, SyncData = JsonConvert.SerializeObject(data) });
            }

            // 写入园记录 
            var entity = new TicketInPark()
            {
                ParkId = args.ParkId,
                Barcode = args.VerifyCode,
                TicketClassId = args.TicketClassId,
                Qty = args.Persons,
                TerminalId = args.Terminal,
                Remark = args.Remark
            };

            //确保释放对象，防止内存泄漏
            using (var ticketInParkRepository = IocManager.Instance.ResolveAsDisposable<IRepository<TicketInPark, long>>())
            {
                ticketInParkRepository.Object.Insert(entity);
            }
        }
    }
}