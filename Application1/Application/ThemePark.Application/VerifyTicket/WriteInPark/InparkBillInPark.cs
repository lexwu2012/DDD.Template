using Abp.Dependency;
using Abp.Domain.Repositories;
using ThemePark.Core.InPark;
using ThemePark.EntityFramework;

namespace ThemePark.Application.VerifyTicket.WriteInPark
{
    internal class InparkBillInPark : WriteInParkBase
    {
        public override WriteInParkJobArgs GetJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            var inParkPerson = ticketCheckData.AllowPersons - noPast;
            ticketCheckData.InPersons += inParkPerson;

            InParkBillState inParkBillState = InParkBillState.Valid;
            if (ticketCheckData.InPersons == ticketCheckData.BillPersons) // 都入园了
            {
                ticketCheckData.CheckState = CheckState.Used;
                inParkBillState = InParkBillState.Entered;
            }
            else
            {
                ticketCheckData.CheckState = CheckState.Idle;
            }

            // ticketCheckData.CommonTicketDto.Persons = ticketCheckData.BillPersons - ticketCheckData.InPersons;
            ticketCheckData.InparkBillDto.Persons = ticketCheckData.BillPersons - ticketCheckData.InPersons;
            ticketCheckData.AllowPersons = ticketCheckData.InparkBillDto.Persons;

            // 更新表数据
            // 构造写入园记录作业的参数
            var jobArgs = new WriteInParkJobArgs
            {
                Terminal = terminal,
                VerifyType = ticketCheckData.VerifyType,
                VerifyCode = id,
                //获取本地公园编号
                ParkId = LocalParkId,
                InParkCount = ticketCheckData.InPersons,
                Persons = inParkPerson,
                TicketSaleStatus = (int)inParkBillState,
                Remark = remark
            };

            return jobArgs;
        }
        public override void UpdateDB(WriteInParkJobArgs args)
        {
            string sql = "update InParkBill set InparkCounts=" + args.InParkCount
                + " ,InParkBillState=" + args.TicketSaleStatus
                + " ,LastModificationTime=getdate() where Id='" + args.VerifyCode + "'";

            //确保释放对象，防止内存泄漏
            using (var context = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
            {
                context.Object.Database.ExecuteSqlCommand(sql);
            }


            //确保释放对象，防止内存泄漏
            using (var repos1 = IocManager.Instance.ResolveAsDisposable<IRepository<VisitorInPark, long>>())
            {
                // 写入园记录
                var entity = new VisitorInPark()
                {
                    ParkId = args.ParkId,
                    Barcode = args.VerifyCode,
                    Persons = args.Persons,
                    Remark = args.Remark,
                    TerminalId = args.Terminal
                };
                repos1.Object.Insert(entity);
            }

        }

    }
}