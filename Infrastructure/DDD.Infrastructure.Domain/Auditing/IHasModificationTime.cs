using System;

namespace DDD.Infrastructure.Domain.Auditing
{
    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; set; }
    }
}
