using DDD.Domain.Common.Repositories;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Repositories;
using DDD.Domain.Service.CheckoffAutoAcp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DDD.Infrastructure.Web.Helper;
using Newtonsoft.Json;

namespace DDD.Domain.Service.CheckoffAutoAcp
{
    public class CheckoffAutoAcpDomainService: DomainServiceBase, ICheckoffAutoAcpDomainService
    {
        private readonly ICheckoffAutoAcpRepository _checkoffAutoAcpRepository;
        //private readonly IRepositoryWithEntity<Core.Model.CheckoffAutoAcp> _checkoffAutoAcpRepository;
        private readonly IRepositoryWithEntity<CheckoffMonitor> _checkoffMonitorRepository;

        public CheckoffAutoAcpDomainService(//IRepositoryWithEntity<Core.Model.CheckoffAutoAcp> checkoffAutoAcpRepository,
            ICheckoffAutoAcpRepository checkoffAutoAcpRepository,
            IRepositoryWithEntity<CheckoffMonitor> checkoffMonitorRepository)
        {
            _checkoffAutoAcpRepository = checkoffAutoAcpRepository;
            _checkoffMonitorRepository = checkoffMonitorRepository;
        }

        public void SendEmail(int hour)
        {
            string subject = string.Empty;
            string emailBody = string.Empty;

            var entitys = _checkoffMonitorRepository.AsNoTracking()
                .Where(m => DbFunctions.DiffDays(m.CreationTime, DateTime.Now) > 0 && DbFunctions.DiffDays(DateTime.Now.AddDays(1), m.CreationTime) > 0).
                OrderBy(m => m.CreationTime)
                .Select(m => new { m.Id, m.CheckoffType, m.CheckoffModule, m.CheckoffTotal, m.FinishFlag, m.Remark, m.LastModificationTime, m.CreationTime});

            if (entitys.Any())
            {
                subject = string.Format("{0}_{1}点代扣监控邮件", DateTime.Now.ToString("yyyyMMdd"), hour);

                if (hour == 8)
                {
                    var eightList = _checkoffAutoAcpRepository.GetCheckoffAutoAcpsByType(8);
                    if (eightList.Any())
                    {
                        emailBody += "</br></br>微信端疑似重复入帐监控</br></br>";
                        emailBody += JArrayHelper.JsonToHtmlJArray(JsonConvert.SerializeObject(eightList), "");
                    }
                    else
                    {
                        emailBody += "</br></br>监控未发现异常入帐</br></br>";
                    }

                    var tenList = _checkoffAutoAcpRepository.GetCheckoffAutoAcpsByType(10);

                    if (tenList.Any())
                    {
                        emailBody += "</br></br>今日服务器插入统计</br></br>";
                        emailBody += JArrayHelper.JsonToHtmlJArray(JsonConvert.SerializeObject(tenList), "");
                    }
                }
            }
            else
            {
                subject = string.Format("[紧急]{0}代扣监控邮件", DateTime.Now.ToString("yyyyMMdd"));
                emailBody = string.Format("当前时间未找到任何相关记录!");
            }

        }
    }
}
