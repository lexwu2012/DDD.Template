using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DDD.Infrastructure.Web.Threading;
using DDD.Schedule.App_Start;
using DDD.Schedule.Jobs;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace DDD.Schedule
{
    class Program
    {
        static void Main(string[] args)
        {
            //初始化
            Startup.Configuration();

            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();

                //x.SetServiceName(ScheduleSettings.Instance.ServiceName);
                //x.SetDisplayName(ScheduleSettings.Instance.ServiceDisplayName);
                //x.SetDescription(ScheduleSettings.Instance.ServiceDescription);

                x.SetServiceName("TestJob服务名称");
                x.SetDisplayName("TestJob服务显示名称");
                x.SetDescription("TestJob服务描述");

                x.Service<Bootstrapper>(s =>
                {
                    s.ConstructUsing(name => new Bootstrapper());
                    s.WhenStarted(w => w.Start());
                    s.WhenStopped(w => w.Stop());
                });

                //x.SetStartTimeout(TimeSpan.FromMinutes(5));
                //x.SetStopTimeout(TimeSpan.FromMinutes(35));
                //x.EnableServiceRecovery(r => { r.RestartService(1); });
                x.OnException((ex) =>
                {
                    WriteExceptionLog(ex);
                    Console.WriteLine(ex.ToString() ?? ex.InnerException.ToString());
                    Console.WriteLine(ex.StackTrace ?? ex.InnerException?.StackTrace);
                });
            });

            //try
            //{
            //    // Grab the Scheduler instance from the Factory 
            //    IScheduler scheduler = AsyncHelper.RunSync( () => StdSchedulerFactory.GetDefaultScheduler());

            //    // and start it off
            //    scheduler.Start();

            //    // define the job and tie it to our HelloJob class
            //    IJobDetail job = JobBuilder.Create<CheckoffAutoAcpSendEmailJob>()
            //        .WithIdentity("job1", "group1")
            //        .Build();

            //    // Trigger the job to run now, and then repeat every 10 seconds
            //    ITrigger trigger = TriggerBuilder.Create()
            //        .WithIdentity("trigger1", "group1")
            //        .StartNow()
            //        .WithSimpleSchedule(x => x
            //            .WithIntervalInSeconds(10)
            //            .RepeatForever())
            //        .Build();

            //    // Tell quartz to schedule the job using our trigger
            //    scheduler.ScheduleJob(job, trigger);

            //    // some sleep to show what's happening
            //    Thread.Sleep(TimeSpan.FromSeconds(60));

            //    // and last shut down the scheduler when you are ready to close your program
            //    scheduler.Shutdown();
            //}
            //catch (SchedulerException se)
            //{
            //    Console.WriteLine(se);
            //}

            //Console.WriteLine("Press any key to close the application");
            //Console.ReadKey();

            //Console.ReadKey();
        }


        private static void WriteExceptionLog(Exception ex)
        {
            var domainPath = AppDomain.CurrentDomain.BaseDirectory;
            var logPath = $@"{domainPath}App_Data\GlobalException";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            var fileName = DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
            var message = $"{ex.ToString() ?? ex.InnerException.ToString()}\r\n{ex.StackTrace ?? ex.InnerException.StackTrace}\r\n";
            var bytes = Encoding.Default.GetBytes(message);
            using (FileStream fileStream = new FileStream(Path.Combine(logPath, fileName), FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.Position = fileStream.Length;
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
        }
    }

}
