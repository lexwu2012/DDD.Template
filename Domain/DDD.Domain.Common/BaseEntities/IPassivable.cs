namespace DDD.Domain.Common.BaseEntities
{
    public interface IPassivable
    {
        bool IsActive { get; set; }
    }
}
