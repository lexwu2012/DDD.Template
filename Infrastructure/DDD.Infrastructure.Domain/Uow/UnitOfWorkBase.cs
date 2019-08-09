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

        #region EventHandler（这里用来执行资源释放）

        public event EventHandler Completed;
        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        public event EventHandler Disposed;

        #endregion

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

        /// <summary>
        /// 开启真正事务
        /// </summary>
        /// <param name="options"></param>
        public virtual void Begin(UnitOfWorkOptions options)
        {
            //告知已经进入事务中
            PreventMultipleBegin();

            Options = options;

            //让子类开启真正事务
            BeginUow();
        }

        /// <summary>
        /// 同步方法真正的提交事务
        /// </summary>
        public void Complete()
        {
            try
            {
                //提交事务
                CompleteUow();
                _succeed = true;
                //提交完事务后的后续动作（释放当前的uow）
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <summary>
        /// 异步方法真正的提交事务
        /// </summary>
        /// <returns></returns>
        public async Task CompleteAsync()
        {
            try
            {
                //提交事务
                await CompleteUowAsync();
                _succeed = true;
                //提交完事务后的后续动作（释放当前的uow，在UnitOfWorkManager开启第一个uow时指定了Completed事件（委托），将Current设为null）
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <summary>
        /// 事务的真正释放方法，由于这个方法重写了IDisposable，所以在using之后会调用这个方法
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
                //这里应该使用领域事件来触发抛出
                throw _exception;
            }
            
            //在子类释放dbcontext
            DisposeUow();

            //释放uow
            OnDisposed();
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
        /// 提交事务后的一个后续动作（Current置为空）
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed.InvokeSafely(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void OnDisposed()
        {
            Disposed.InvokeSafely(this);
        }

        /// <summary>
        /// 避免多线程开启多个真正事务
        /// </summary>
        private void PreventMultipleBegin()
        {
            if (_isBeginCalledBefore)
            {
                throw new Exception("This unit of work has started before. Can not call Start method more than once.");
            }

            _isBeginCalledBefore = true;
        }

        #region abstract methods

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

        #endregion

    }
}
