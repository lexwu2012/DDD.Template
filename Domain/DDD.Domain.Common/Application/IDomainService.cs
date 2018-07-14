using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Common.Application
{
    /// <summary>
    /// domain的标志
    /// </summary>
    public interface IDomainService: ITransientDependency
    {
    }
}
