using DDD.Domain.Entities;

namespace DDD.Domain.Auditing
{
    public interface IDeletionAudited : IHasDeletionTime
    {
        long? DeleterUserId { get; set; }
    }

    public interface IDeletionAudited<TUser> : IDeletionAudited
        where TUser : IEntity<long>
    {
        TUser DeleterUser { get; set; }
    }
}
