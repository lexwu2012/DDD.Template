using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Uow
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public abstract void SaveChanges();

        public bool IsCommitted { get; set; }

        public abstract int Commit();

        public abstract void Rollback();
    }
}
