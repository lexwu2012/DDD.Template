namespace DDD.Infrastructure.Domain.Uow
{
    public interface IUnitOfWork : IUnitOfWorkCompleteHandle, IActiveUnitOfWork
    {
        bool IsCommitted { get; }

        int Commit();

        void Rollback();

        void Begin(UnitOfWorkOptions options);

        IUnitOfWork Outer { get; set; }
    }
}
