using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Infrastructure.Domain.Auditing
{
    public interface IModificationAudited : IHasModificationTime
    {
        long? LastModifierUserId { get; set; }
    }
    
    public interface IModificationAudited<TUser> : IModificationAudited
        where TUser : IEntity<long>
    {
        TUser LastModifierUser { get; set; }
    }
}
