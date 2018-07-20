using System.Threading.Tasks;

namespace DDD.Infrastructure.Domain.Events.Handlers
{
    public interface IAsyncEventHandler<in TEventData> : IEventHandler
    {
        /// <summary>
        /// Handler handles the event by implementing this method.
        /// </summary>
        /// <param name="eventData">Event data</param>
        Task HandleEventAsync(TEventData eventData);
    }
}
