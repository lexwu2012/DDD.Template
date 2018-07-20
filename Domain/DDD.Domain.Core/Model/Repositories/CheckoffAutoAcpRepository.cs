using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Repositories;

namespace DDD.Domain.Core.Model.Repositories
{
    public class CheckoffAutoAcpRepository : DDDRepositoryWithDbContext<CheckoffAutoAcp>, ICheckoffAutoAcpRepository
    {
        public CheckoffAutoAcpRepository(IDbContextProvider<DDDDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public List<CheckoffAutoAcp> GetCheckoffAutoAcpsByType(int type)
        {
            List<CheckoffAutoAcp> lists = new List<CheckoffAutoAcp>();

            //switch (type)
            //{
            //    case 0:
            //        {
            //            if (GetAll().Where(m => DbFunctions.DiffDays(m.CreationTime, DateTime.Now) > 0).Any())
            //            {
            //                lists = GetAll()
            //                 .Where(m => DbFunctions.DiffDays(m.CreationTime, DateTime.Now) > 0)
            //                 .OrderBy(m => m.PayStatus)
            //                 .GroupBy(m => new { m.MyAccCode, m.PayStatus })
            //                 .SelectMany(m => m.Select(o => new CheckoffAutoAcp { o.MyAccCode = m.Key.MyAccCode }));
            //            }
            //        }                   
            //        break;
            //}
            
            return lists;
        }
    }
}
