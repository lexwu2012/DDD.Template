using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Service.Reader.Interfaces
{
    public interface IReaderDomainService: ITransientDependency
    {
        Result LendBook(int readId, string bookId, string comment);
    }
}
