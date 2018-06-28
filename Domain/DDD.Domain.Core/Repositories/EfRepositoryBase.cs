using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Entities;
using DDD.Domain.Uow;

namespace DDD.Domain.Core.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : RepositoryBase<TEntity, TPrimaryKey>, IRepositoryWithDbContext
        where TEntity : class ,IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        public IUnitOfWork UnitOfWork { get; set; }

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        public virtual TDbContext Context { get; set; }

        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        //public virtual TDbContext Context => _dbContextProvider.GetDbContext();

        //public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
        //{
        //    _dbContextProvider = dbContextProvider;
        //}

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
            //AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public override TEntity Insert(TEntity entity)
        {
            return Table.Add(entity);
        }

        public override void Delete(TEntity entity)
        {
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

    }

    public class EfRepositoryBase<TDbContext, TEntity> : EfRepositoryBase<TDbContext, TEntity, int>,
        IRepositoryWithEntity<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {
    }
    
}
