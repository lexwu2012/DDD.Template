using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DDD.Domain.Common.Uow;
using DDD.Domain.Uow;

namespace DDD.Domain.Core.Uow
{
    public class EfUnitOfWork : UnitOfWorkBase
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

        protected override void CompleteUow()
        {
            throw new System.NotImplementedException();
        }
    }
}
