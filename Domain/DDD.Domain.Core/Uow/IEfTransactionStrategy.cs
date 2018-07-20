using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Core.Uow
{
    public interface IEfTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver, string schema)
            where TDbContext : DbContext;

        void Commit();

        void Dispose(IIocResolver iocResolver);
    }
}
