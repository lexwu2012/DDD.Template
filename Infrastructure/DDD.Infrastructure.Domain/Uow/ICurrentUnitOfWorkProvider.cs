namespace DDD.Infrastructure.Domain.Uow
{
    public interface ICurrentUnitOfWorkProvider
    {
        IUnitOfWork Current { get; set; }
    }
}
