using System.Transactions;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// 对外提供的uow接口
    /// </summary>
    public interface IUnitOfWorkManager
    {
        IActiveUnitOfWork Current { get; }

        IUnitOfWorkCompleteHandle Begin();

     
        IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope);
      
        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);
    }
}
