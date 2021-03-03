using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using ThemePark.Common;
using ThemePark.Infrastructure.Core;

namespace ThemePark.Infrastructure.EntityFramework
{
    /// <summary>
    /// Class BaseMap.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
    /// <seealso cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{TEntity}"/>
    public abstract class BaseMap<TEntity, TPrimaryKey> : EntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity<TPrimaryKey>, new()
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMap{TEntity, TPrimaryKey}"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="hasIdentityKey">if set to <c>true</c> [has identity key].</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected BaseMap(string table, bool hasIdentityKey = true)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentNullException(nameof(table));
            }

            //todo: virtual method call in constructor
            //Initializing();

            this.ToTable(table);

            this.HasKey(o => o.Id);

            var primaryType = typeof(TPrimaryKey);
            if (primaryType.IsValueType)
            {
                if (hasIdentityKey)
                {
                    if (primaryType == typeof(int))
                    {
                        var lambda = ExpressionHelper.CreatePropertySelectorLambda<TEntity, IEntity<int>, int>(o => o.Id);
                        this.Property((Expression<Func<TEntity, int>>) lambda)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    }
                    if (primaryType == typeof(long))
                    {
                        var lambda =
                            ExpressionHelper.CreatePropertySelectorLambda<TEntity, IEntity<long>, long>(o => o.Id);
                        this.Property((Expression<Func<TEntity, long>>) lambda)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    }
                }
                //else
                //{
                //    if (primaryType == typeof(int))
                //    {
                //        var lambda = ExpressionHelper.CreatePropertySelectorLambda<TEntity, IEntity<int>, int>(o => o.Id);
                //        this.Property((Expression<Func<TEntity, int>>)lambda).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                //    }
                //    if (primaryType == typeof(long))
                //    {
                //        var lambda =
                //            ExpressionHelper.CreatePropertySelectorLambda<TEntity, IEntity<long>, long>(o => o.Id);
                //        this.Property((Expression<Func<TEntity, long>>)lambda).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                //    }
                //}
            }

            var type = typeof(TEntity);
            if (typeof(IRowVersion).IsAssignableFrom(type))
            {
                var lambda = ExpressionHelper.CreatePropertySelectorLambda<TEntity, IRowVersion, byte[]>(o => o.Timespan);
                if (lambda != null)
                {
                    this.Property((Expression<Func<TEntity, byte[]>>)lambda).IsRowVersion();
                }
            }

            //add ParkPermission Index
            if (typeof(IParkPermission).IsAssignableFrom(type))
            {
                var lambda = ExpressionHelper.CreatePropertySelectorLambda<TEntity, IParkPermission, int>(o => o.ParkId);

                this.Property((Expression<Func<TEntity, int>>)lambda).HasColumnAnnotation("Index",
                        new IndexAnnotation(new IndexAttribute()));
                //"Ix_" + table + "_" + nameof(IParkPermission.ParkId)
            }

            //add AgencyPermission Index
            if (typeof(IAgencyPermission).IsAssignableFrom(type))
            {
                var lambda =
                    ExpressionHelper.CreatePropertySelectorLambda<TEntity, IAgencyPermission, int>(o => o.AgencyId);

                this.Property((Expression<Func<TEntity, int>>)lambda).HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute()));
            }

            //Initialized();
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Initializeds this instance.
        /// </summary>
        protected virtual void Initialized()
        {
        }

        /// <summary>
        /// Initializings this instance.
        /// </summary>
        protected virtual void Initializing()
        {
        }

        #endregion Methods
    }

    /// <summary>
    /// Class BaseMap.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="ThemePark.Infrastructure.EntityFramework.BaseMap{TEntity, System.Int32}"/>
    public abstract class BaseMap<TEntity> : BaseMap<TEntity, int> where TEntity : class, IEntity<int>, new()
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMap{TEntity}"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="hasIdentityKey">if set to <c>true</c> [has identity key].</param>
        protected BaseMap(string table, bool hasIdentityKey = true) : base(table, hasIdentityKey)
        {
        }

        #endregion Ctor
    }
}