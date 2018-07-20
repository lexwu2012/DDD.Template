namespace DDD.Infrastructure.Domain.Uow
{
    public interface IActiveUnitOfWork
    {
        UnitOfWorkOptions Options { get; }

        bool IsDisposed { get; }


        void SaveChanges();
    }
}
