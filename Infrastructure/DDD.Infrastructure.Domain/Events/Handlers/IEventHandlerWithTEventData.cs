namespace DDD.Infrastructure.Domain.Events.Handlers
{
    public interface IEventHandlerWithTEventData<in TEventData> : IEventHandler
    {
      
        void HandleEvent(TEventData eventData);
    }
}
