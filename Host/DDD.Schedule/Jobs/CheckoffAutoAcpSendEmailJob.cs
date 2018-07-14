using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using Quartz;

namespace DDD.Schedule.Jobs
{
    public class CheckoffAutoAcpSendEmailJob : IJob
    {
        private readonly ICheckoffAutoAcpEmailAppService _checkoffAutoAcpEmailAppService;

        public CheckoffAutoAcpSendEmailJob(ICheckoffAutoAcpEmailAppService checkoffAutoAcpEmailAppService)
        {
            _checkoffAutoAcpEmailAppService = checkoffAutoAcpEmailAppService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _checkoffAutoAcpEmailAppService.SendEmail();
            //return Task.Run(() => Console.WriteLine(DateTime.Now.ToString() + "\t工作太久了，休息10分钟吧。"));
        }
    }
}
