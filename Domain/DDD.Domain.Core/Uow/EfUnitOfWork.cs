using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using DDD.Infrastructure.Domain.DbHelper;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Core.Uow
{
    public class EfUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        public DbContext DbContext { get; set; }
        protected IDictionary<string, DbContext> ActiveDbContexts { get; }

        private readonly IDbContextResolver _dbContextResolver;

        protected IIocResolver IocResolver { get; }

        protected TransactionScope CurrentTransaction { get; set; }

        protected List<DbContext> DbContexts { get; }
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;
        private readonly IEfTransactionStrategy _transactionStrategy;

        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="defaultOptions"></param>
        /// <param name="iocResolver"></param>
        /// <param name="currentUnitOfWork"></param>
        /// <param name="dbContextResolver"></param>
        /// <param name="dbContextTypeMatcher"></param>
        /// <param name="connectionStringResolver"></param>
        /// <param name="transactionStrategy"></param>
        public EfUnitOfWork(IUnitOfWorkDefaultOptions defaultOptions,
            IIocResolver iocResolver,
            ICurrentUnitOfWorkProvider currentUnitOfWork,
            IDbContextResolver dbContextResolver, 
            IDbContextTypeMatcher dbContextTypeMatcher,
            IConnectionStringResolver connectionStringResolver, IEfTransactionStrategy transactionStrategy)
            : base(defaultOptions, currentUnitOfWork, connectionStringResolver)
        {
            IocResolver = iocResolver;
            _dbContextResolver = dbContextResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
            _transactionStrategy = transactionStrategy;
            ActiveDbContexts = new Dictionary<string, DbContext>();
            DbContexts = new List<DbContext>();
        }


        public override int Commit()
        {
            if (IsCommitted)
            {
                return 0;
            }
            try
            {
                int result = DbContext.SaveChanges();
                IsCommitted = true;
                return result;
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public override void Rollback()
        {
            IsCommitted = false;
        }

        /// <summary>
        /// 创建事务
        /// </summary>
        protected override void BeginUow()
        {
            if (Options.IsTransactional == true)
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

                //这里应该解析出来的是TransactionScopeEfTransactionStrategy
                //_transactionStrategy.InitOptions(Options);
            }
            
        }

        /// <summary>
        /// 保存数据并且提交事务
        /// </summary>
        protected override void CompleteUow()
        {
            SaveChanges();

            if (CurrentTransaction == null)
            {
                return;
            }

            CurrentTransaction.Complete();

            CurrentTransaction.Dispose();
            CurrentTransaction = null;
        }

        public override void SaveChanges()
        {
            //DbContext.SaveChanges();
            foreach (var dbContext in ActiveDbContexts.Values.ToImmutableList())
            {
                dbContext.SaveChanges();
            }
        }

        public virtual TDbContext GetOrCreateDbContext<TDbContext>()
            where TDbContext : DbContext
        {
            //todo: 区分oracle和sqlserver数据库
           
            //var connectionString = "DefaultConnection";

            var concreteDbContextType = _dbContextTypeMatcher.GetConcreteType(typeof(TDbContext));

            string connectionString;
            string schema = null;
            if (DbConnectionHelper.DbCatagory == DBType.Oracle.ToString())
            {
                connectionString = ResolveConnectionString(ref schema);
            }
            else
            {
                connectionString = ResolveConnectionString();
            }
            var dbContextKey = concreteDbContextType.FullName + "#" + connectionString;

            DbContext dbContext;
            if (!ActiveDbContexts.TryGetValue(dbContextKey, out dbContext))
            {
                if (Options.IsTransactional == true)
                {
                    //解析出来的是TransactionScopeEfTransactionStrategy
                    dbContext = _transactionStrategy.CreateDbContext<TDbContext>(connectionString, _dbContextResolver, schema);                    
                }
                else
                {
                    dbContext = _dbContextResolver.Resolve<TDbContext>(connectionString);
                }

                DbContexts.Add(dbContext);

                //if (Options.Timeout.HasValue && !dbContext.Database.CommandTimeout.HasValue)
                //{
                //    dbContext.Database.CommandTimeout = Options.Timeout.Value.TotalSeconds.To<int>();
                //}

                ((IObjectContextAdapter)dbContext).ObjectContext.ObjectMaterialized += (sender, args) =>
                {
                    ObjectContext_ObjectMaterialized(dbContext, args);
                };

                ActiveDbContexts[dbContextKey] = dbContext;
            }

            return (TDbContext)dbContext;
        }

        /// <summary>
        /// uow完成后自动释放
        /// </summary>
        protected override void DisposeUow()
        {
            //DbContext.Dispose();
            
            if(null == Options)
                return;

            if (Options.IsTransactional == true)
            {
                foreach (var dbContext in DbContexts)
                {
                    IocResolver.Release(dbContext);
                }

                DbContexts.Clear();

                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
            else
            {
                foreach (var activeDbContext in ActiveDbContexts.Values.ToImmutableList())
                {
                    Release(activeDbContext);
                }
            }

            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }

            ActiveDbContexts.Clear();
        }

        private static void ObjectContext_ObjectMaterialized(DbContext dbContext, ObjectMaterializedEventArgs e)
        {
            var entityType = ObjectContext.GetObjectType(e.Entity.GetType());

            dbContext.Configuration.AutoDetectChangesEnabled = false;
            var previousState = dbContext.Entry(e.Entity).State;
            
            dbContext.Entry(e.Entity).State = previousState;
            dbContext.Configuration.AutoDetectChangesEnabled = true;
        }

        protected virtual void Release(DbContext dbContext)
        {
            dbContext.Dispose();
            IocResolver.Release(dbContext);
        }
    }
}
