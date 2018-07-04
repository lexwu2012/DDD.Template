using System;
using DDD.Domain.Uow;

namespace DDD.Domain.Common.Uow
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {

        private bool _succeed;

        private Exception _exception;

        public abstract void SaveChanges();

        public bool IsCommitted { get; set; }

        public virtual int Commit()
        {
            try
            {
                CompleteUow();
                IsCommitted = true;
                return 1;
                //OnCompleted();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Rollback()
        {
            IsCommitted = false;
        }

        public void Begin(UnitOfWorkOptions options)
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            try
            {
                CompleteUow();
                _succeed = true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        protected abstract void CompleteUow();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
