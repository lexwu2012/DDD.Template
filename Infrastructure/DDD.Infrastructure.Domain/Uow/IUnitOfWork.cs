namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// 工作单元，基于接口分离原则，把方法分到了IUnitOfWork，IUnitOfWorkCompleteHandle，IActiveUnitOfWork这三个不同的接口
    /// </summary>
    public interface IUnitOfWork : IUnitOfWorkCompleteHandle, IActiveUnitOfWork
    {
        bool IsCommitted { get; }

        int Commit();

        void Rollback();

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="options"></param>
        void Begin(UnitOfWorkOptions options);

        /// <summary>
        /// 保存第一个开启的uow
        /// </summary>
        IUnitOfWork Outer { get; set; }
    }
}
