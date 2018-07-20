using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Application;

namespace DDD.Application.Service.CheckoffAutoAcp.Interfaces
{
    public interface ICheckoffAutoAcpEmailAppService : IApplicationService
    {
        void SendEmail();
    }
}
