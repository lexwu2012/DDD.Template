namespace DDD.Domain.Common.BaseEntities
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
