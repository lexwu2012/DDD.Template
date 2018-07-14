using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace DDD.Schedule.App_Start
{

    public class Bootstrapper : ServiceControl
    {
        private IScheduler _scheduler;

        public Bootstrapper(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public bool Start(HostControl hostControl)
        {
            if (!_scheduler.IsStarted)
            {
                _scheduler.Start();
            }
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (!_scheduler.IsStarted)
            {
                return true;
            }
            _scheduler.Shutdown(true);
            return true;
        }
    }
}
