using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Domain.Uow
{
    public class UnitOfWorkFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Exception that caused failure.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkFailedEventArgs"/> object.
        /// </summary>
        /// <param name="exception">Exception that caused failure</param>
        public UnitOfWorkFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
