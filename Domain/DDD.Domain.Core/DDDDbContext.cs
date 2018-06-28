using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Castle.Core.Logging;
using DDD.Domain.Core.Model;
using DDD.Domain.Entities;
using EntityFramework.DynamicFilters;

namespace DDD.Domain.Core
{
    /// <summary>
    /// DbContext
    /// </summary>
    public class DDDDbContext : DbContext
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

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Filter("SoftDelete", (ISoftDelete d) => d.IsDeleted, false);

            modelBuilder.Configurations.AddFromAssembly(typeof(DDDDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            try
            {
                var result = base.SaveChanges();
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }


        protected virtual void LogDbEntityValidationException(DbEntityValidationException exception)
        {
            Logger.Error("There are some validation errors while saving changes in EntityFramework:");
            foreach (var ve in exception.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
            {
                Logger.Error(" - " + ve.PropertyName + ": " + ve.ErrorMessage);
            }
        }
    }
}
