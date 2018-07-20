using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Application;

namespace DDD.Domain.Service.CheckoffAutoAcp.Interfaces
{
    public interface ICheckoffAutoAcpDomainService:IDomainService
    {
        void SendEmail(int hour);
    }
}
