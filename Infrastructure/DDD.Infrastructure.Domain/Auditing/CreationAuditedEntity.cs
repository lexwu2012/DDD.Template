using System;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Infrastructure.Domain.Auditing
{
    public abstract class CreationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, ICreationAudited
    {
        protected CreationAuditedEntity()
        {
            CreationTime = DateTime.Now;
        }
        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
    }
}
