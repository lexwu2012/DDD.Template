using System;
using DDD.Domain.Uow;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Common.Uow
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
