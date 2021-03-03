using Abp.Dependency;
using Abp.Domain.Repositories;
using ThemePark.Core.InPark;

namespace ThemePark.Application.VerifyTicket.WriteInPark
{
    internal class VIPCardInPark : WriteInParkBase
    {
        public override WriteInParkJobArgs GetJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            ticketCheckData.CheckState = CheckState.Idle;
            var inParkPerson = ticketCheckData.AllowPersons - noPast;

            // 更新表数据
            // 构造写入园记录作业的参数
            var jobArgs = new WriteInParkJobArgs
            {
                Terminal = terminal,
                VerifyType = ticketCheckData.VerifyType,
                VerifyCode = ticketCheckData.VerifyCode,
                ParkId = LocalParkId,
                Id = ticketCheckData.VIPCardDto.VipCardId.ToString(),
                Persons = inParkPerson,
                Remark = remark
            };
            return jobArgs;
        }
        public override void UpdateDB(WriteInParkJobArgs args)
        {
            //确保释放对象，防止内存泄漏
            using (var repos = IocManager.Instance.ResolveAsDisposable<IRepository<VIPInPark, long>>())
            {
                var entity = new VIPInPark()
                {
                    ParkId = args.ParkId,
                    TerminalId = args.Terminal,
                    Remark = args.Remark,
                    VipCardId = long.Parse(args.Id)
                };
                repos.Object.Insert(entity);
            }
        }

    }
}