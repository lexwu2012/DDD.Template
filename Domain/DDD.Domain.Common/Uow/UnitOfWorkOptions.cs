using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace DDD.Domain.Uow
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
        
        
    }
}
