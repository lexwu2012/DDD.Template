using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Schedule.Jobs;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace DDD.Schedule.App_Start
{

    public class Bootstrapper
    {
        private IScheduler _scheduler { get; set; }

        //public Bootstrapper(IScheduler scheduler)
        //{
        //    _scheduler = scheduler;
        //}

        public async Task<bool> Start()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            if (!_scheduler.IsStarted)
            {
                await _scheduler.Start();

                IJobDetail job = JobBuilder.Create<CheckoffAutoAcpSendEmailJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(3)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await _scheduler.ScheduleJob(job, trigger);
            }
            return true;
        }

        public bool Stop()
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
