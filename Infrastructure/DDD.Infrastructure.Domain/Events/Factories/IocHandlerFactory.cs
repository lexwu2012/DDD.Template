using System;
using DDD.Infrastructure.Domain.Events.Handlers;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Domain.Events.Factories
{
    public class IocHandlerFactory : IEventHandlerFactory
    {
        public Type HandlerType { get; }

        private readonly IIocResolver _iocResolver;

        public IocHandlerFactory(IIocResolver iocResolver, Type handlerType)
        {
            _iocResolver = iocResolver;
            HandlerType = handlerType;
        }

      
        public IEventHandler GetHandler()
        {
            return (IEventHandler)_iocResolver.Resolve(HandlerType);
        }

        public Type GetHandlerType()
        {
            return HandlerType;
        }

      
        public void ReleaseHandler(IEventHandler handler)
        {
            _iocResolver.Release(handler);
        }
    }
}
