using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Infrastructure.Domain.Auditing
{
    public interface IAudited : ICreationAudited, IModificationAudited
    {
    }

    public interface IAudited<TUser> : IAudited, ICreationAudited<TUser>, IModificationAudited<TUser>
        where TUser : IEntity<long>
    {

    }
}
