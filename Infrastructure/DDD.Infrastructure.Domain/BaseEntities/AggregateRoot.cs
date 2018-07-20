namespace DDD.Infrastructure.Domain.BaseEntities
{

    public class AggregateRoot : AggregateRoot<int>, IAggregateRoot
    {

    }

    public class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {
        //public virtual ICollection<IEventData> DomainEvents { get; }

        public AggregateRoot()
        {
            
        }

        public string Remark { get; set; }
    }
}
