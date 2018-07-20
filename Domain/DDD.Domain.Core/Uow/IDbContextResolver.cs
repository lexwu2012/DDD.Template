using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Core.Uow
{
    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string connectionString, string dbSchema = null)
            where TDbContext : DbContext;

        TDbContext Resolve<TDbContext>(DbConnection existingConnection, bool contextOwnsConnection)
            where TDbContext : DbContext;
    }
}
