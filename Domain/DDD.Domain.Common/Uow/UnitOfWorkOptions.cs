using System;
using System.Collections.Generic;
using System.Transactions;
using DDD.Domain.Uow;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace DDD.Domain.Common.Uow
{
    public class UnitOfWorkOptions
    {
        public UnitOfWorkOptions()
        {
            FilterOverrides = new List<DataFilterConfiguration>();
        }

        public TransactionScopeOption? Scope { get; set; }
        
        public bool? IsTransactional { get; set; }
        
        public TimeSpan? Timeout { get; set; }
        
        public IsolationLevel? IsolationLevel { get; set; }
        
        public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }
        
        public List<DataFilterConfiguration> FilterOverrides { get; }

        internal void FillDefaultsForNonProvidedOptions(IUnitOfWorkDefaultOptions defaultOptions)
        {
            if (!IsTransactional.HasValue)
            {
                IsTransactional = defaultOptions.IsTransactional;
            }

            if (!Scope.HasValue)
            {
                Scope = defaultOptions.Scope;
            }

            if (!Timeout.HasValue && defaultOptions.Timeout.HasValue)
            {
                Timeout = defaultOptions.Timeout.Value;
            }

            if (!IsolationLevel.HasValue && defaultOptions.IsolationLevel.HasValue)
            {
                IsolationLevel = defaultOptions.IsolationLevel.Value;
            }
        }
    }
}
