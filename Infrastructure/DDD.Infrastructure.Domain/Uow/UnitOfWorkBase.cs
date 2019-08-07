using DDD.Infrastructure.Domain.Events.Extensions;
using System;
using System.Threading.Tasks;
using Castle.Core;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// uow基类，完成基本的方法，主要在子类完成业务动作
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {

        private bool _succeed;

        private Exception _exception;

        public event EventHandler Completed;
        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        public event EventHandler Disposed;

        protected IUnitOfWorkDefaultOptions DefaultOptions { get; }

        protected ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; }

        protected IConnectionStringResolver ConnectionStringResolver { get; }

        public UnitOfWorkOptions Options { get; private set; }

        private bool _isBeginCalledBefore;

        public bool IsCommitted { get; set; }

        public bool IsDisposed { get; private set; }

        [DoNotWire]
        public IUnitOfWork Outer { get; set; }

        protected UnitOfWorkBase(
            IUnitOfWorkDefaultOptions defaultOptions, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IConnectionStringResolver connectionStringResolver)
        {
            DefaultOptions = defaultOptions;
            CurrentUnitOfWorkProvider = currentUnitOfWorkProvider;
            ConnectionStringResolver = connectionStringResolver;
        }

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

        public virtual void Begin(UnitOfWorkOptions options)
        {
            //告知已经进入事务中，且对释放有影响
            PreventMultipleBegin();

            Options = options; 
            
            BeginUow();
        }

        /// <summary>
        /// 同步事务完成动作（提交事务）
        /// </summary>
        public void Complete()
        {
            try
            {
                //提交事务
                CompleteUow();
                _succeed = true;
                //释放当前的uow
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }
        /// <summary>
        /// 异步事务完成动作（提交事务）
        /// </summary>
        /// <returns></returns>
        public async Task CompleteAsync()
        {
            try
            {
                await CompleteUowAsync();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!_isBeginCalledBefore || IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!_succeed)
            {
                throw _exception;
            }
            

            DisposeUow();
        }

        protected virtual string ResolveConnectionString()
        {
            return "Default";
        }

        protected virtual string ResolveConnectionString(ref string schema)
        {
            return ConnectionStringResolver.GetNameOrConnectionString(ref schema);
        }

        /// <summary>
        /// 提交事务后的一个后续动作（释放资源）
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed.InvokeSafely(this);
        }

        private void PreventMultipleBegin()
        {
            if (_isBeginCalledBefore)
            {
                throw new Exception("This unit of work has started before. Can not call Start method more than once.");
            }

            _isBeginCalledBefore = true;
        }

        /// <summary>
        /// 让子类实现自己的完成动作
        /// </summary>
        protected abstract void BeginUow();

        /// <summary>
        /// 让子类实现自己的完成动作
        /// </summary>
        protected abstract void CompleteUow();

        /// <summary>
        /// 让子类实现自己的完成动作
        /// </summary>
        protected abstract Task CompleteUowAsync();

        /// <summary>
        /// 让子类实现自己的完成动作
        /// </summary>
        protected abstract void DisposeUow();

        /// <summary>
        /// 让子类实现自己的完成动作
        /// </summary>
        public abstract void SaveChanges();

        /// <summary>
        /// 让子类实现自己的完成动作
        /// </summary>
        public abstract Task SaveChangesAsync();
    }
}
