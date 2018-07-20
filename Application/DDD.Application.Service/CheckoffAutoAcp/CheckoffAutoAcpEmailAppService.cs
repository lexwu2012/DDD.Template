using System;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Domain.Service.CheckoffAutoAcp.Interfaces;

namespace DDD.Application.Service.CheckoffAutoAcp
{
    public class CheckoffAutoAcpEmailAppService : AppServiceBase, ICheckoffAutoAcpEmailAppService
    {
        private readonly ICheckoffAutoAcpDomainService _checkoffAutoAcpDomainService;
        private bool flag8 = false;
        private bool flag10 = false;
        private bool flag13 = false;
        private bool flag15 = false;
        private bool flag17 = false;
        private bool flag19 = false;

        public CheckoffAutoAcpEmailAppService(ICheckoffAutoAcpDomainService checkoffAutoAcpDomainService)
        {
            _checkoffAutoAcpDomainService = checkoffAutoAcpDomainService;
        }

        public void SendEmail()
        {
            DateTime now = DateTime.Now;
            int hour = now.Hour;
            int minute = now.Minute;
            switch (hour)
            {
                case 8:
                    _checkoffAutoAcpDomainService.SendEmail(hour);
                    break;
                case 10:
                    _checkoffAutoAcpDomainService.SendEmail(hour);
                    break;
                case 13:
                    _checkoffAutoAcpDomainService.SendEmail(hour);
                    break;
                case 15:
                    _checkoffAutoAcpDomainService.SendEmail(hour);
                    break;
                case 17:
                    _checkoffAutoAcpDomainService.SendEmail(hour);
                    break;
                case 19:
                    if (minute > 30)
                    {
                        _checkoffAutoAcpDomainService.SendEmail(hour);
                    }
                    
                    break;
                default:
                    flag8 = false;
                    flag10 = false;
                    flag13 = false;
                    flag15 = false;
                    flag17 = false;
                    flag19 = false;
                    Logger.Info("代扣邮件统计未到时间");
                    break;
            }

            Logger.Info(string.Format("{0}点代扣邮件统计开始!", hour.ToString()));

        }
    }
}
