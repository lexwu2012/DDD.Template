using System;
using DDD.Infrastructure.Domain.Events.Handlers;

namespace DDD.Infrastructure.Domain.Events.Factories
{
    public class TransientEventHandlerFactory<THandler> : IEventHandlerFactory
       where THandler : IEventHandler, new()
    {
       
        public IEventHandler GetHandler()
        {
            return new THandler();
        }

        public Type GetHandlerType()
        {
            return typeof(THandler);
        }

  
        public void ReleaseHandler(IEventHandler handler)
        {
            if (handler is IDisposable)
            {
                (handler as IDisposable).Dispose();
            }
        }
    }
}
