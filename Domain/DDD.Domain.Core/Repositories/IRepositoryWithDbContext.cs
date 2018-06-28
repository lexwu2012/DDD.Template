using System.Data.Entity;

namespace DDD.Domain.Core.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
    }
}
