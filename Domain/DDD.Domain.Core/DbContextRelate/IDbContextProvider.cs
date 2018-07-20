using System.Data.Entity;

namespace DDD.Domain.Core.DbContextRelate
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();        
    }
}
