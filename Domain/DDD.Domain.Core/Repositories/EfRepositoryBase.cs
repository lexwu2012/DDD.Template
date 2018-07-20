using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DDD.Domain.Core.DbContextRelate;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Domain.Uow;

namespace DDD.Domain.Core.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : RepositoryBase<TEntity, TPrimaryKey>, IRepositoryWithDbContext
        where TEntity : class, IAggregateRoot<TPrimaryKey> 
        where TDbContext : DbContext
    {
        public IUnitOfWork UnitOfWork { get; set; }

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        //public virtual TDbContext Context { get; set; }

        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        public virtual TDbContext Context => _dbContextProvider.GetDbContext();

        public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public DbContext GetDbContext()
        {
            return Context;
        }

        #region Methods should relate to dbcontext

        public override IQueryable<TEntity> GetAll()
        {
            return Table;
        }

        public override TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public override TEntity Insert(TEntity entity)
        {
            //Context.Entry(entity).State = EntityState.Added;
            //return entity;
            return Table.Add(entity);
        }

        public override void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public override void Delete(TPrimaryKey id)
        {
            var entity = Table.Local.FirstOrDefault(ent => EqualityComparer<TPrimaryKey>.Default.Equals(ent.Id, id));
            if (entity == null)
            {
                entity = FirstOrDefault(id);
                if (entity == null)
                {
                    return;
                }
            }

            Delete(entity);
        }

        public override TPrimaryKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);

            Context.SaveChanges();

            return entity.Id;
        }

        #endregion

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }

    }

    public class EfRepositoryBase<TDbContext, TEntity> : EfRepositoryBase<TDbContext, TEntity, int>,
        IRepositoryWithEntity<TEntity>
        where TEntity : class, IAggregateRoot
        where TDbContext : DbContext
    {
        public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
    
}
