using System;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// 当前活动的工作单元，包括Completed，Disposed，Failed事件代理，Filter的enable和disable,以及同步、异步的SaveChanges方法
    /// </summary>
    public interface IActiveUnitOfWork
    {
        UnitOfWorkOptions Options { get; }

        bool IsDisposed { get; }

        /// <summary>
        /// 工作单元提交之后执行的事件代理
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// 工作单元执行失败后执行的事件代理
        /// </summary>
        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <summary>
        /// 释放资源的事件代理
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// 
        /// </summary>
        void SaveChanges();
    }
}
