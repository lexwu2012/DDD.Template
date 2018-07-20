using System;
using DDD.Infrastructure.Domain.Events.Handlers;

namespace DDD.Infrastructure.Domain.Events.Factories
{
    public interface IEventHandlerFactory
    {
      
        IEventHandler GetHandler();

     
        Type GetHandlerType();

     
        void ReleaseHandler(IEventHandler handler);
    }
}
