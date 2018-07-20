using System;
using DDD.Infrastructure.Common.Reflection;
using DDD.Infrastructure.Domain.Events.Handlers;

namespace DDD.Infrastructure.Domain.Events.Factories
{
    public class SingleInstanceHandlerFactory : IEventHandlerFactory
    {
       
        public IEventHandler HandlerInstance { get; private set; }

        public SingleInstanceHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandler GetHandler()
        {
            return HandlerInstance;
        }

        public Type GetHandlerType()
        {
            return ProxyHelper.UnProxy(HandlerInstance).GetType();
        }

        public void ReleaseHandler(IEventHandler handler)
        {

        }
    }
}
