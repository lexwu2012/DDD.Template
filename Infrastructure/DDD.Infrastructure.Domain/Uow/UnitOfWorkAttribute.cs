using System;
using System.Transactions;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// uow特性，用于标志需要用到uow方法
    /// </summary>
    public class UnitOfWorkAttribute : Attribute
    {
        public bool? IsTransactional { get; set; }

        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 事务隔离标志
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Default: false.
        /// </summary>
        public bool IsDisabled { get; set; }

      
        public UnitOfWorkAttribute()
        {

        }

        public UnitOfWorkAttribute(bool isTransactional)
        {
            IsTransactional = isTransactional;
        }


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
