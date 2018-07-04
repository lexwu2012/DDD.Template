using System;
using DDD.Domain.Uow;

namespace DDD.Domain.Common.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        bool IsCommitted { get; }

        int Commit();

        void Rollback();

        void Begin(UnitOfWorkOptions options);

        void Complete();
    }
}
