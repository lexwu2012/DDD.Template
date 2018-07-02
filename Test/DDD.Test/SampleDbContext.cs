using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.CustomAttributes;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Core;
using DDD.Domain.Core.Repositories;

namespace DDD.Test
{
    [AutoRepositoryTypes(
            typeof(IRepositoryWithEntity<>),
            typeof(IRepositoryWithTEntityAndTPrimaryKey<,>),
            typeof(DDDRepositoryWithDbContext<>),
            typeof(DDDRepositoryWithDbContext<,>)
            )]
    public class SampleDbContext : DDDDbContext
    {
        public SampleDbContext()
        {

        }

        public SampleDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public SampleDbContext(DbConnection connection)
            : base(connection, false)
        {

        }
    }
}
