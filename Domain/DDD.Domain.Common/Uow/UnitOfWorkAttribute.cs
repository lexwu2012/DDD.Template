using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DDD.Domain.Uow;

namespace DDD.Domain.Common.Uow
{
    public class UnitOfWorkAttribute : Attribute
    {

        /// <summary>
        /// Is this UOW transactional?
        /// Uses default value if not supplied.
        /// </summary>
        public bool? IsTransactional { get; set; }

        /// <summary>
        /// Timeout of UOW As milliseconds.
        /// Uses default value if not supplied.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this UOW is transactional, this option indicated the isolation level of the transaction.
        /// Uses default value if not supplied.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Used to prevent starting a unit of work for the method.
        /// If there is already a started unit of work, this property is ignored.
        /// Default: false.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Creates a new UnitOfWorkAttribute object.
        /// </summary>
        public UnitOfWorkAttribute()
        {

        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// </summary>
        /// <param name="isTransactional">
        /// Is this unit of work will be transactional?
        /// </param>
        public UnitOfWorkAttribute(bool isTransactional)
        {
            IsTransactional = isTransactional;
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// </summary>
        /// <param name="timeout">As milliseconds</param>
        public UnitOfWorkAttribute(int timeout)
        {
            Timeout = TimeSpan.FromMilliseconds(timeout);
        }

        internal UnitOfWorkOptions CreateOptions()
        {
            return new UnitOfWorkOptions
            {
                IsTransactional = IsTransactional,
                IsolationLevel = IsolationLevel,
                Timeout = Timeout
            };
        }

    }
}
