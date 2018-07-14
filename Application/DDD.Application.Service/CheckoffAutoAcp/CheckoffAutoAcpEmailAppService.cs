using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Domain.Core.Model.Repositories.Interfaces;

namespace DDD.Application.Service.CheckoffAutoAcp
{
    public class CheckoffAutoAcpEmailAppService : AppServiceBase, ICheckoffAutoAcpEmailAppService
    {
        private readonly ICheckoffAutoAcpRepository _checkoffAutoAcpRepository;

        public CheckoffAutoAcpEmailAppService(ICheckoffAutoAcpRepository checkoffAutoAcpRepository)
        {
            _checkoffAutoAcpRepository = checkoffAutoAcpRepository;
        }

        public Task SendEmail()
        {
            throw new NotImplementedException();
        }
    }
}
