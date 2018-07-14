using DDD.Domain.Entities;

namespace DDD.Domain.BaseEntities
{
    public interface IAggregateRoot : IAggregateRoot<int>, IEntity
    {

    }

    public interface IAggregateRoot<TPrimaryKey> : IEntity<TPrimaryKey>
    {

        /// <summary>
        /// 每个聚合根的备注
        /// </summary>
        string Remark { get; set; }
    }
}
