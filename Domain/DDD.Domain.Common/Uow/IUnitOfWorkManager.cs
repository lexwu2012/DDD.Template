using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DDD.Domain.Uow;

namespace DDD.Domain.Common.Uow
{
    public interface IUnitOfWorkManager
    {
        IActiveUnitOfWork Current { get; }

        IUnitOfWorkCompleteHandle Begin();

     
        IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope);
      
        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);
    }
}
