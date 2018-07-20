namespace DDD.Infrastructure.Domain.BaseEntities
{
    public interface IPassivable
    {
        bool IsActive { get; set; }
    }
}
