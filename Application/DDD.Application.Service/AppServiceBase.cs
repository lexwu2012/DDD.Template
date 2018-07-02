using Castle.Core.Logging;

namespace DDD.Application.Service
{
    public abstract class AppServiceBase
    {
        public ILogger Logger { protected get; set; }

        protected AppServiceBase()
        {
            Logger = NullLogger.Instance;
        }
    }
}
