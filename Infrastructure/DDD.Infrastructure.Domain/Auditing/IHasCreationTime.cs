using System;

namespace DDD.Infrastructure.Domain.Auditing
{
    public interface IHasCreationTime
    {
        DateTime CreationTime { get; set; }
    }
}
