using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DDD.Infrastructure.Domain.DbHelper;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Core.Uow
{
    public class TransactionScopeEfTransactionStrategy : IEfTransactionStrategy, ITransientDependency
    {
        protected UnitOfWorkOptions Options { get; private set; }

        protected TransactionScope CurrentTransaction { get; set; }

        protected List<DbContext> DbContexts { get; }

        public TransactionScopeEfTransactionStrategy()
        {
            DbContexts = new List<DbContext>();
        }

        #region Public Methods

        /// <summary>
        ///  初始化事务
        /// </summary>
        /// <param name="options"></param>
        public virtual void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;

            //开启真正的TransactionScope事务
            StartTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void Commit()
        {
            if (CurrentTransaction == null)
            {
                return;
            }

            //事务提交
            CurrentTransaction.Complete();

            //手动释放Transaction
            CurrentTransaction.Dispose();

            CurrentTransaction = null;
        }

        public DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver, string schema)
            where TDbContext : DbContext
        {
            var dbContext = dbContextResolver.Resolve<TDbContext>(connectionString, schema);
            DbContexts.Add(dbContext);
            return dbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        public virtual void Dispose(IIocResolver iocResolver)
        {
            foreach (var dbContext in DbContexts)
            {
                iocResolver.Release(dbContext);
            }

            DbContexts.Clear();

            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 创建事务
        /// </summary>
        private void StartTransaction()
        {
            if (CurrentTransaction != null)
            {
                return;
            }

            TransactionOptions transactionOptions;

            //oracle只支持ReadCommitted和Serializable
            if (DbConnectionHelper.DbCatagory == DBType.Oracle.ToString())
            {
                transactionOptions = new TransactionOptions
                {
                    IsolationLevel = Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadCommitted),
                };
            }
            else
            {
                transactionOptions = new TransactionOptions
                {
                    IsolationLevel = Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted),
                };
            }

            if (Options.Timeout.HasValue)
            {
                transactionOptions.Timeout = Options.Timeout.Value;
            }

            CurrentTransaction = new TransactionScope(
                Options.Scope.GetValueOrDefault(TransactionScopeOption.Required),
                transactionOptions,
                Options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled)
            );
        }

        #endregion

    }
}
