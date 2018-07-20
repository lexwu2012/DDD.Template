using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Core.Model.Repositories.Interfaces
{
    public interface IBookRepository : IRepositoryWithTEntityAndTPrimaryKey<Book, string>, ITransientDependency
    {
    }
}
