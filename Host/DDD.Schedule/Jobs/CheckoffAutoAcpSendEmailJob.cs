using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Logger;
using Quartz;

namespace DDD.Schedule.Jobs
{
    public class CheckoffAutoAcpSendEmailJob : IJob, ISingletonDependency
    {
        private readonly ICheckoffAutoAcpEmailAppService _checkoffAutoAcpEmailAppService;
        private readonly ILogger _logger;

        public CheckoffAutoAcpSendEmailJob(ICheckoffAutoAcpEmailAppService checkoffAutoAcpEmailAppService)
        {
            _checkoffAutoAcpEmailAppService = checkoffAutoAcpEmailAppService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            LogHelper.Logger.Info("test");
            return Task.Run(() => Console.WriteLine(DateTime.Now.ToString() + "\t工作太久了，休息10分钟吧。"));
            
        }
            //await _checkoffAutoAcpEmailAppService.SendEmail();
    }
}
