using System;
using DDD.Domain.Auditing;
using DDD.Domain.Entities;

namespace DDD.Domain.Common.Auditing
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
