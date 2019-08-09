using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Domain.Uow
{
    public class InnerUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        private volatile bool _isCompleteCalled;
        private volatile bool _isDisposed;

        /// <summary>
        /// 内嵌uow的同步提交（不做真正事务的处理）
        /// </summary>
        public void Complete()
        {
            _isCompleteCalled = true;
        }

        /// <summary>
        /// 内嵌uow的异步提交（不做真正事务的处理）
        /// </summary>
        /// <returns></returns>
        public Task CompleteAsync()
        {
            _isCompleteCalled = true;
            return Task.FromResult(0);
        }

        /// <summary>
        /// 内嵌uow的释放（using执行完），如果没有执行Complete（_isCompleteCalled == false）,则
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            if (!_isCompleteCalled)
            {
                if (HasException())
                {
                    return;
                }

                //没有执行Complete，又没有异常抛出，则说明在业务层面中，调用uow的次序发生问题
                throw new Exception("Did not call Complete method of a unit of work.");
            }
        }

        private static bool HasException()
        {
            try
            {
                return Marshal.GetExceptionCode() != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }        
    }
}
