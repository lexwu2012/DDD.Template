namespace DDD.Infrastructure.Domain.BaseEntities
{

    public interface IEntity : IEntity<int>
    {

    }

    public interface IEntity<TPrimaryKey>
    {        
        /// <summary>
        /// 每个实体的主键（不包括值类型）
        /// </summary>
        TPrimaryKey Id { get; set; }


        bool IsTransient();
    }
}
