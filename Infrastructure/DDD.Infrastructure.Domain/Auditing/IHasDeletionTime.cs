using System;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Infrastructure.Domain.Auditing
{
    public interface IHasDeletionTime : ISoftDelete
    {
        DateTime? DeletionTime { get; set; }
    }
}
