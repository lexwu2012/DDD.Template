using DDD.Domain.Common.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.CheckoffAutoAcp.Interfaces
{
    public interface ICheckoffAutoAcpDomainService:IDomainService
    {
        void SendEmail(int hour);
    }
}
