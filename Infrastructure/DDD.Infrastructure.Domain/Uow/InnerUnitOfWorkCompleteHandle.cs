using System;
using System.Runtime.InteropServices;

namespace DDD.Infrastructure.Domain.Uow
{
    public class InnerUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        private volatile bool _isCompleteCalled;
        private volatile bool _isDisposed;


        public void Complete()
        {
            _isCompleteCalled = true;
        }

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
