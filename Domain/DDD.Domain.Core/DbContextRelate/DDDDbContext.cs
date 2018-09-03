using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Castle.Core.Logging;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Common.Reflection;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Domain.CustomAttributes;
using DDD.Infrastructure.Domain.DbHelper;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Ioc.Dependency;
using Oracle.ManagedDataAccess.Client;

namespace DDD.Domain.Core.DbContextRelate
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
    //[DbConfigurationType(typeof(DDDDbConfiguration))]
    public class DDDDbContext : DbContext, ITransientDependency
    {
        #region Tables

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<CheckoffAutoAcp> CheckoffAutoAcp { get; set; }

        public virtual DbSet<CheckoffCommand> CheckoffCommand { get; set; }

        //public virtual DbSet<Blog> Blogs { get; set; }
        //public virtual DbSet<Post> Posts { get; set; }
        //public virtual DbSet<Book> Books { get; set; }
        #endregion

        public ILogger Logger { get; set; }

        private static string Schema { get; set; }

        public DDDDbContext() : base("Default")
        {
            InitializeDbContext();
        }

        public DDDDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            InitializeDbContext();
        }

        public DDDDbContext(string nameOrConnectionString, string schema) : base("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=FTdevdb02.dafycredit.com)(PORT=1521))(CONNECT_DATA=(SERVER=dedicated)(SERVICE_NAME=devdb02)));User ID=dafy_sales2;Password=Test$20150508;Connect Timeout=60;")
        {
            Schema = schema;
            InitializeDbContext();
        }

      
        protected DDDDbContext(DbCompiledModel model) : base(model)
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

            if (DbConnectionHelper.DbCatagory == DBType.Oracle.ToString())
            {
                Schema = Schema ?? "DAFY_SALES2";
                modelBuilder.HasDefaultSchema(Schema);
            }            

            //modelBuilder.Filter("SoftDelete", (ISoftDelete d) => d.IsDeleted, false);

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
            catch(Exception ex)
            {
                throw ex;
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
