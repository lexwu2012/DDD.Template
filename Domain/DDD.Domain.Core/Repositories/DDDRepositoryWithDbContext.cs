using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DDD.Domain.BaseEntities;
using DDD.Domain.Common.DbHelper;
using DDD.Domain.Entities;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Common.Extensions;

namespace DDD.Domain.Core.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class DDDRepositoryWithDbContext<TEntity, TPrimaryKey> : EfRepositoryBase<DDDDbContext, TEntity, TPrimaryKey>, IDDDRepository<TEntity, TPrimaryKey>
        where TEntity : class, IAggregateRoot<TPrimaryKey>
    {
        //public DDDRepositoryWithDbContext() : base()
        //{
        //    string schema = string.Empty;
        //    var connectionStr = InitConnectStr(ref schema);
        //    //this.Context = new DDDDbContext(connectionStr, schema);
        //}

        public DDDRepositoryWithDbContext(IDbContextProvider<DDDDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        //public override TEntity Insert(TEntity entity)
        //{
        //    base.Insert(entity);
        //    //Context.SaveChanges();

        //    return entity;
        //}

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Table.AddRange(entities);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            using (DisableAutoDetectChanges())
            {
                entities.ForEach(o =>
                {
                    AttachIfNot(o);
                    Context.Entry(o).State = EntityState.Modified;
                });
            }
        }

        #region Private Methods

        private IDisposable DisableAutoDetectChanges()
        {
            var old = Context.Configuration.AutoDetectChangesEnabled;
            Context.Configuration.AutoDetectChangesEnabled = false;

            return new DisposeAction(() => Context.Configuration.AutoDetectChangesEnabled = old);
        }
        #endregion


    }

    //public class DDDRepositoryWithDbContext<TEntity> : DDDRepositoryWithDbContext<TEntity, int>
    //    where TEntity : class, IAggregateRoot
    //{
    //    #region Constructors

    //    public DDDRepositoryWithDbContext()
    //        : base()
    //    {
    //    }

    //    #endregion Constructors

    //    //do not add any method here, add to the class above (since this inherits it)
    //}

    public class DDDRepositoryWithDbContext<TEntity> : DDDRepositoryWithDbContext<TEntity, int>, IDDDRepository<TEntity>
       where TEntity : class, IAggregateRoot
    {
        #region Constructors

        public DDDRepositoryWithDbContext(IDbContextProvider<DDDDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        #endregion Constructors

        //do not add any method here, add to the class above (since this inherits it)
    }
}
