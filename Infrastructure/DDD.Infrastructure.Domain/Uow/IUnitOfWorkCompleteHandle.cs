using System;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// 定义UOW同步和异步的complete方法。这个接口的实现类InnerUnitOfWorkCompleteHandle不会真正提交事务，只是做一个标志，真正的事务提交在第一个开启的事务里面提交
    /// </summary>
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        /// <summary>
        /// 同步
        /// </summary>
        void Complete();

        /// <summary>
        /// 异步
        /// </summary>
        /// <returns></returns>
        Task CompleteAsync();
    }
}
