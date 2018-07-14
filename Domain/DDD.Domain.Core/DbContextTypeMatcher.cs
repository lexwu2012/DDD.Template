using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.Uow;
using DDD.Domain.Core.Uow;

namespace DDD.Domain.Core
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<DDDDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}
