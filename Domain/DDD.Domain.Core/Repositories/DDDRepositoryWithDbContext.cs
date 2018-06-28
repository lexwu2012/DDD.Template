using DDD.Domain.Entities;

namespace DDD.Domain.Core.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class DDDRepositoryWithDbContext<TEntity, TPrimaryKey> : EfRepositoryBase<DDDDbContext, TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        public DDDRepositoryWithDbContext() 
            : base()
        {
            this.Context = new DDDDbContext();
        }
    }

    public class DDDRepositoryWithDbContext<TEntity> : DDDRepositoryWithDbContext<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        #region Constructors

        public DDDRepositoryWithDbContext()
            : base()
        {
        }

        #endregion Constructors

        //do not add any method here, add to the class above (since this inherits it)
    }
}
