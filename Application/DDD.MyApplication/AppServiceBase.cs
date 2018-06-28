using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;

namespace DDD.MyApplication
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
