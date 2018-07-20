using System;

namespace DDD.Infrastructure.Domain.Events
{
    public interface IEventData
    {
        /// <summary>
        /// 事件发生事件
        /// </summary>
        DateTime EventTime { get; set; }

        /// <summary>
        /// 事件源
        /// </summary>
        object EventSource { get; set; }
    }
}
