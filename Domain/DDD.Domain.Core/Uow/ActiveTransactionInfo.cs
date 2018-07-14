using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Core.Uow
{
    public class ActiveTransactionInfo
    {
        public DbContextTransaction DbContextTransaction { get; }

        public DbContext StarterDbContext { get; }

        public List<DbContext> AttendedDbContexts { get; }

        public ActiveTransactionInfo(DbContextTransaction dbContextTransaction, DbContext starterDbContext)
        {
            DbContextTransaction = dbContextTransaction;
            StarterDbContext = starterDbContext;

            AttendedDbContexts = new List<DbContext>();
        }
    }
}
