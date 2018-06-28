using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Uow
{
    public interface IUnitOfWork
    {
        bool IsCommitted { get; }

        int Commit();

        void Rollback();
    }
}
