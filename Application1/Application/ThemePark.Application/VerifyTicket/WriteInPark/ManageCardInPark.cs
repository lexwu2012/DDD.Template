using Abp.Dependency;
using Abp.Domain.Repositories;
using ThemePark.Core.InPark;

namespace ThemePark.Application.VerifyTicket.WriteInPark
{
    internal class ManageCardInPark : WriteInParkBase
    {
        public override WriteInParkJobArgs GetJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            ticketCheckData.CheckState = CheckState.Idle;

            //本次入园人数
            var inParkPerson = ticketCheckData.AllowPersons - noPast;

            // 更新表数据
            // 构造写入园记录作业的参数
            var jobArgs = new WriteInParkJobArgs
            {
                Terminal = terminal,
                VerifyType = ticketCheckData.VerifyType,
                VerifyCode = ticketCheckData.VerifyCode,
                //获取本地公园编号
                ParkId = LocalParkId,
                Id = ticketCheckData.ManageCardDto.Id,
                Persons = inParkPerson,
                Remark = remark
            };

            return jobArgs;
        }
        public override void UpdateDB(WriteInParkJobArgs args)
        {
            //确保释放对象，防止内存泄漏
            using (var repos = IocManager.Instance.ResolveAsDisposable<IRepository<ManagerCardInPark, long>>())
            {
                var entity = new ManagerCardInPark()
                {
                    ParkId = args.ParkId,
                    TerminalId = args.Terminal,
                    Persons = args.Persons,
                    Remark = args.Remark,
                    ManageCardInfoId = int.Parse(args.Id)
                };
                repos.Object.Insert(entity);
            }
        }

    }
}