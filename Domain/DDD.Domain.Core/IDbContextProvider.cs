using System.Data.Entity;

namespace DDD.Domain.Core
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();        
    }
}
