namespace DDD.Infrastructure.Domain.Uow
{
    public interface IConnectionStringResolver
    {
        string GetNameOrConnectionString(ref string schema);
    }
}
