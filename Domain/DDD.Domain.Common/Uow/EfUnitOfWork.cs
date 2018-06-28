using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Uow
{
    public class EfUnitOfWork: UnitOfWorkBase
    {
        public DbContext DbContext { get; set; }

        public override void SaveChanges()
        {
            DbContext.SaveChanges();
        }
        
        public override int Commit()
        {
            if (IsCommitted)
            {
                return 0;
            }
            try
            {
                int result = DbContext.SaveChanges();
                IsCommitted = true;
                return result;
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public override void Rollback()
        {
            IsCommitted = false;
        }
    }
}
