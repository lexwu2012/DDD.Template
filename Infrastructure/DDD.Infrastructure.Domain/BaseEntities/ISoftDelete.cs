namespace DDD.Infrastructure.Domain.BaseEntities
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
