using System;
using Abp.Dependency;
using Abp.Domain.Repositories;
using ThemePark.Application.VerifyTicket.Finger;
using ThemePark.Core.ReEnter;
using ThemePark.EntityFramework;
using ThemePark.FaceClient;

namespace ThemePark.Application.VerifyTicket.WriteInPark
{
    public class SecondTicketInPark : WriteInParkBase
    {
        public override WriteInParkJobArgs GetJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            var inParkPerson = ticketCheckData.AllowPersons - noPast;

            if (inParkPerson > 0)
            {
                ticketCheckData.CheckState = CheckState.Used;
                ////当日已使用指纹的移除缓存，防止中控直接验证指纹
                //List<FingerCache.FingerDataItem> listFc;
                //FingerCache.DicFinger.TryRemove(id, out listFc);
                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(id.ToUpper());
                FaceApi.RemoveUser(FaceCatalog.Ticket, long.Parse(id));
            }
       
            // 更新表数据
            // 构造写入园记录作业的参数
            var jobArgs = new WriteInParkJobArgs
            {
                Terminal = terminal,
                VerifyType = ticketCheckData.VerifyType,
                VerifyCode = id,
                ParkId = LocalParkId,
                Persons = inParkPerson,
                ReEnterEnrollId = Convert.ToInt32(ticketCheckData.SecondTicketDto.EnrollId),
                Remark = remark
            };
            return jobArgs;
        }

        public override void UpdateDB(WriteInParkJobArgs args)
        {
            #region 更新二次入园票状态

            string sql = "update ReEnterEnroll set ReEnterEnrollState=2 , LastModificationTime=getdate() where Id='" +
                         args.ReEnterEnrollId + "'";

            //确保释放对象，防止内存泄漏
            using (var context = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
            {
                context.Object.Database.ExecuteSqlCommand(sql);
            }

            #endregion

            // 写入园记录
            ReEnterInPark entity = new ReEnterInPark
            {
                ReEnterEnrollId = args.ReEnterEnrollId,
                Persons = args.Persons,
                TerminalId = args.Terminal,
                Remark = args.Remark
            };

            //确保释放对象，防止内存泄漏
            using (var ticketInParkRepository =
                IocManager.Instance.ResolveAsDisposable<IRepository<ReEnterInPark, long>>())
            {
                ticketInParkRepository.Object.InsertAndGetId(entity);
            }

        }

    }
}
