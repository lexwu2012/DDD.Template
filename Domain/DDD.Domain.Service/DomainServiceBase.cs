using Castle.Core.Logging;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Service
{
    public abstract class DomainServiceBase
    {
        private IUnitOfWork _unitOfWork;

       
        public ILogger Logger { protected get; set; }
    }
}
