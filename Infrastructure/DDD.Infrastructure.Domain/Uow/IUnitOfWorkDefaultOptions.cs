using System;
using System.Collections.Generic;
using System.Transactions;

namespace DDD.Infrastructure.Domain.Uow
{
    public interface IUnitOfWorkDefaultOptions
    {
        bool IsTransactional { get; set; }

        List<Func<Type, bool>> ConventionalUowSelectors { get; }

        /// <summary>
        /// Scope option.
        /// </summary>
        TransactionScopeOption Scope { get; set; }

        /// <summary>
        /// Gets/sets a timeout value for unit of works.
        /// </summary>
        TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets/sets isolation level of transaction.
        /// This is used if <see cref="IsTransactional"/> is true.
        /// </summary>
        IsolationLevel? IsolationLevel { get; set; }
    }
}
