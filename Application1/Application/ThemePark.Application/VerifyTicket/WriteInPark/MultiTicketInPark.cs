using System;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using ThemePark.Application.DataSync;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.VerifyTicket.Finger;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.DataSync;
using ThemePark.Core.InPark;
using ThemePark.Core.ParkSale;
using ThemePark.EntityFramework;
using ThemePark.FaceClient;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Application.VerifyTicket.WriteInPark
{
    internal class MultiTicketInPark : WriteInParkBase
    {
        public override WriteInParkJobArgs GetJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            // 更表缓存数据
            var inParkPerson = ticketCheckData.AllowPersons - noPast;
            ticketCheckData.InPersons += inParkPerson;

            // 入园信息，日期 公园ID 人数，如: 2017-3-15 41 3, 
            string inParkText = DateTime.Today.ToString("yyyy-MM-dd")
                                + " " + LocalParkId + " " + inParkPerson + ",";
            ticketCheckData.InParkInfo += inParkText;

            //每天允许入园人数
            var perday = ticketCheckData.GetInParkRull().InParkTimesPerDay * ticketCheckData.GetPersons();

            //每个公园允许入园人数
            var perpark = ticketCheckData.GetInParkRull().InParkTimesPerPark * ticketCheckData.GetPersons();

            ////今天已入园人数
            //var todayInpark = VerifyTicketHelper.GetTodayInParkTimes(ticketCheckData.InParkInfo);

            //本公园已入园人数
            var localInpark = VerifyTicketHelper.GetParkInParkTimes(LocalParkId, ticketCheckData.InParkInfo);

            if (perpark == localInpark)
            {
                ticketCheckData.CheckState = CheckState.Used;

                //当日已使用指纹的移除缓存，防止中控直接验证指纹
                // List<FingerCache.FingerDataItem> listFc;
                
                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(id.ToUpper());
                //FingerCache.DicFinger.TryRemove(id, out listFc);
                FaceApi.RemoveUser(FaceCatalog.Ticket, long.Parse(id));

                var parks = ticketCheckData.GetCanInParkIds().Trim().Split(',');
                parks = parks.Except(new[] { LocalParkId.ToString() }).ToArray();
                foreach (var park in parks)
                {
                    IocManager.Instance.Resolve<IDataSyncManager>().UploadDataToTargetPark(int.Parse(park), new DataSyncInput()
                    {
                        SyncType = DataSyncType.MultiTicketEnrollPhotoRemove,
                        SyncData = JsonConvert.SerializeObject(new MultiTicketPhotoRemoveDto(id))
                    });
                }
            }
            else
            {
                ticketCheckData.CheckState = CheckState.Idle;
            }

            //套票入园人数始终为1
            ticketCheckData.MultiTicketDto.Persons = 1;

            ticketCheckData.AllowPersons = ticketCheckData.MultiTicketDto.Persons;

            // 更新表数据
            // 构造写入园记录作业的参数
            var jobArgs = new WriteInParkJobArgs
            {
                Terminal = terminal,
                VerifyType = ticketCheckData.VerifyType,
                VerifyCode = id,
                ParkId = LocalParkId,
                TicketSaleStatus = ticketCheckData.CheckState == CheckState.Used ? (int)TicketSaleStatus.InPark : (int)TicketSaleStatus.Valid,
                TableName = ticketCheckData.TableName,
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
            #region 更新TOTicket、NonGroupTicket或GroupTicket表

            if (string.IsNullOrWhiteSpace(args.TableName))
                return;

            if (args.Persons > 0)
            {
                // 入园信息，日期 公园ID 人数，如: 2017-3-15 41 3, 
                string inParkText = DateTime.Today.ToString("yyyy-MM-dd")
                                    + " " + args.ParkId + " " + args.Persons + ",";

                string sql = "update " + args.TableName + " set InparkCounts=" + args.InParkCount
                             + ", TicketSaleStatus=" + args.TicketSaleStatus
                             + ", Remark=isnull(Remark,'')+" + "'" + inParkText + "'"
                             + ", LastModificationTime=getdate() where Id='" + args.VerifyCode + "'";

                //确保释放对象，防止内存泄漏
                using (var context = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
                {
                    context.Object.Database.ExecuteSqlCommand(sql);
                }
            }

            #endregion

            // 如果是OTA的订单票，同步中心订单数据
            if (args.TableName == typeof(TOTicket).Name)
            {
                // ToDo ...
            }

            var dto = new MultiTicketInparkDto();
            if (args.TableName == typeof(TOTicket).Name)
            {

                dto.TicketClassType = TicketCategory.Order;
            }
            else if (args.TableName == typeof(NonGroupTicket).Name)
            {
                dto.TicketClassType = TicketCategory.NonGroupTicket;
            }
            else
            {
                dto.TicketClassType = TicketCategory.GroupTicket;
            }

            dto.Barcode = args.VerifyCode;
            dto.FromParkid = args.ParkId;
            dto.InparkTime = System.DateTime.Now;
            dto.Persons = args.Persons;
            dto.TicketSaleStatus = (TicketSaleStatus)args.TicketSaleStatus;
            dto.TerminalId = args.Terminal;
            dto.Remark= args.Remark;


            var syncInput = new DataSyncInput
            {
                SyncType = DataSyncType.MultiTicketInpark,
                SyncData = JsonConvert.SerializeObject(dto)
            };
            var otherParkIds = args.InParkIdFilter.Trim().Split(',');

            foreach (var parkid in otherParkIds)
            {
                if (parkid != args.ParkId.ToString())
                {
                    var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                }
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