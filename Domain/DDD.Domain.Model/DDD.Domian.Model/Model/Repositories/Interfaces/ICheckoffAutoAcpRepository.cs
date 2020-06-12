using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Repositories;

namespace DDD.Domain.Core.Model.Repositories.Interfaces
{
    public interface ICheckoffAutoAcpRepository : IRepositoryWithEntity<CheckoffAutoAcp>
    {
        List<CheckoffAutoAcp> GetCheckoffAutoAcpsByType(int type);
    }
}
