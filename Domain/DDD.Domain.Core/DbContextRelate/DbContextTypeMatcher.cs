using DDD.Domain.Core.Uow;
using DDD.Infrastructure.Domain.Uow;

namespace DDD.Domain.Core.DbContextRelate
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<DDDDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}
