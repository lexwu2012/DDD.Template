using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DDD.Domain.Common.Application;
using DDD.Domain.Common.Repositories;

namespace DDD.Domain.Common.Uow
{
    public class UnitOfWorkDefaultOptions : IUnitOfWorkDefaultOptions
    {
        public TransactionScopeOption Scope { get; set; }

        public TimeSpan? Timeout { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        public bool IsTransactionScopeAvailable { get; set; }

        public bool IsTransactional { get; set; }

        public List<Func<Type, bool>> ConventionalUowSelectors { get; }


        public UnitOfWorkDefaultOptions()
        {
            IsTransactional = true;
            Scope = TransactionScopeOption.Required;
            IsTransactionScopeAvailable = true;

            //注册IRepository，IApplicationService和IDomainService自动开启事务
            ConventionalUowSelectors = new List<Func<Type, bool>>
            {
                type => typeof(IRepository).IsAssignableFrom(type) ||
                        typeof(IApplicationService).IsAssignableFrom(type)
                        || typeof(IDomainService).IsAssignableFrom(type)
            };
        }
    }
}
