using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Uow;

namespace DDD.Domain.Common.Uow
{
    public interface IActiveUnitOfWork
    {
        UnitOfWorkOptions Options { get; }

        bool IsDisposed { get; }


        void SaveChanges();
    }
}
