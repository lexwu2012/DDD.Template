using System;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// 事务完成的接口
    /// </summary>
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        void Complete();
    }
}
