using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Common.Uow
{
    /// <summary>
    /// 事务完成的接口
    /// </summary>
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        void Complete();
    }
}
