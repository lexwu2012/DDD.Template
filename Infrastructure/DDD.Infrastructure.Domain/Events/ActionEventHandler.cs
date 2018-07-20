using System;
using DDD.Infrastructure.Domain.Events.Handlers;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Domain.Events
{
    internal class ActionEventHandler<TEventData> :
        IEventHandlerWithTEventData<TEventData>,
        ITransientDependency
    {
        
        public Action<TEventData> Action { get; private set; }

     
        public ActionEventHandler(Action<TEventData> handler)
        {
            Action = handler;
        }

      
        public void HandleEvent(TEventData eventData)
        {
            Action(eventData);
        }
    }
}
