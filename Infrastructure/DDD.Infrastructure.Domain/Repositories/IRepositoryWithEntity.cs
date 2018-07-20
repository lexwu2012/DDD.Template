using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Infrastructure.Domain.Repositories
{
    public interface IRepositoryWithEntity<TEntity> : IRepositoryWithTEntityAndTPrimaryKey<TEntity, int>
        where TEntity : class, IAggregateRoot<int>
    {
    }
}
