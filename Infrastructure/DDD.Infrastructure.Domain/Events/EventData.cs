using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Domain.Events
{
    public abstract class EventData : IEventData
    {
       
        public DateTime EventTime { get; set; }

        public object EventSource { get; set; }

        /// <summary>
        /// cotr
        /// </summary>
        protected EventData()
        {
            EventTime = DateTime.Now;
        }
    }
}
