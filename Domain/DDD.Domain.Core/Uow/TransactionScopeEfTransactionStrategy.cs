﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

        public virtual void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;

            StartTransaction();
        }

        public virtual void Commit()
        {
            if (CurrentTransaction == null)
            {
                return;
            }

            CurrentTransaction.Complete();

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

        private void StartTransaction()
        {
            if (CurrentTransaction != null)
            {
                return;
            }

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted),
            };

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
    }
}
