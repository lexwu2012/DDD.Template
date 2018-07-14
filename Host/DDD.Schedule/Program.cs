using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Schedule.App_Start;
using Topshelf;

namespace DDD.Schedule
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();

                //x.SetServiceName(ScheduleSettings.Instance.ServiceName);
                //x.SetDisplayName(ScheduleSettings.Instance.ServiceDisplayName);
                //x.SetDescription(ScheduleSettings.Instance.ServiceDescription);

                x.SetServiceName("GService.EmailJob");
                x.SetDisplayName("邮件引擎服务");
                x.SetDescription("邮件引擎服务、自动审核报表服务、查询邮件余额、导入OA流程提醒服务等");

                x.Service<Bootstrapper>(s =>
                {
                    s.WhenStarted((w, h) => w.Start(h));
                    s.WhenStopped((w, h) => w.Stop(h));
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

            Console.ReadKey();
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
