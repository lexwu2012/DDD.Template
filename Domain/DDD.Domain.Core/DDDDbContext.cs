﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Castle.Core.Logging;
using DDD.Domain.Common.Auditing;
using DDD.Domain.Common.BaseEntities;
using DDD.Domain.Core.Model;
using DDD.Domain.Entities;
using DDD.Infrastructure.Common.Reflection;
using EntityFramework.DynamicFilters;
using System.Data.Entity.Core.Objects;
using DDD.Domain.Common.CustomAttributes;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Core
{
    /// <summary>
    /// DbContext
    /// </summary>
    [AutoRepositoryTypes(
            typeof(IRepositoryWithEntity<>),
            typeof(IRepositoryWithTEntityAndTPrimaryKey<,>),
            typeof(DDDRepositoryWithDbContext<>),
            typeof(DDDRepositoryWithDbContext<,>)
            )]//注册泛型仓储
    public class DDDDbContext : DbContext, ITransientDependency
    {
        #region Tables

        public virtual DbSet<User> Users { get; set; }
        //public virtual DbSet<Blog> Blogs { get; set; }
        //public virtual DbSet<Post> Posts { get; set; }
        //public virtual DbSet<Book> Books { get; set; }
        #endregion

        public ILogger Logger { get; set; }

        public DDDDbContext() : base("DefaultConnection")
        {
            InitializeDbContext();
        }

        public DDDDbContext(string connectionStr) : base(connectionStr)
        {
            InitializeDbContext();
        }

        public DDDDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, false)
        {
            InitializeDbContext();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            Database.SetInitializer<DDDDbContext>(null);

            modelBuilder.Filter("SoftDelete", (ISoftDelete d) => d.IsDeleted, false);

            modelBuilder.Configurations.AddFromAssembly(typeof(DDDDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            try
            {
                var userId = GetAuditUserId();
                foreach (var entry in ChangeTracker.Entries().ToList())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            Apply4AddedEntity(entry, userId);
                            break;
                        case EntityState.Modified:
                            Apply4ModifiedEntity(entry, userId);
                            break;
                        case EntityState.Deleted:
                            Apply4DeletedEntity(entry, userId);
                            break;
                    }
                }

                var result = base.SaveChanges();
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }


        #region protect Methods

        protected virtual void LogDbEntityValidationException(DbEntityValidationException exception)
        {
            Logger.Error("There are some validation errors while saving changes in EntityFramework:");
            foreach (var ve in exception.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
            {
                Logger.Error(" - " + ve.PropertyName + ": " + ve.ErrorMessage);
            }
        }

        protected void InitializeDbContext()
        {
            ((IObjectContextAdapter)this)
                .ObjectContext
                .ObjectStateManager
                .ObjectStateManagerChanged += ObjectStateManager_ObjectStateManagerChanged;
        }

        protected virtual void ObjectStateManager_ObjectStateManagerChanged(object sender,
           System.ComponentModel.CollectionChangeEventArgs e)
        {
            var contextAdapter = (IObjectContextAdapter)this;
            if (e.Action != CollectionChangeAction.Add)
            {
                return;
            }

            var entry = contextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntry(e.Element);
            switch (entry.State)
            {
                case EntityState.Added:

                    break;
            }
        }

        protected virtual void Apply4AddedEntity(DbEntityEntry entry, long? userId)
        {
            CheckAndSetId(entry.Entity);

            EntityAuditingHelper.SetCreationAuditProperties(entry.Entity, userId);
        }

        protected virtual void Apply4ModifiedEntity(DbEntityEntry entry, long? userId)
        {
            EntityAuditingHelper.SetModificationAuditProperties(entry.Entity, userId);
        }

        protected virtual void Apply4DeletedEntity(DbEntityEntry entry, long? userId)
        {
            EntityAuditingHelper.SetDeletionAuditProperties(entry.Entity, userId);
        }


        protected virtual void CheckAndSetId(object entityAsObj)
        {
            //Set GUID Ids
            var entity = entityAsObj as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                var entityType = ObjectContext.GetObjectType(entityAsObj.GetType());
                var idProperty = entityType.GetProperty("Id");
                var dbGeneratedAttr =
                    ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty);
                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = Guid.NewGuid();
                }
            }
        }

        protected virtual long? GetAuditUserId()
        {
            //todo: 从session里面拿UserId
            return null;
        }

        #endregion
    }
}
