using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Mapping
{
    public abstract class BaseMap<TEntity, TPrimaryKey> : EntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity<TPrimaryKey>, new()
    {
        protected BaseMap(string table, bool hasIdentityKey = true)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentNullException(nameof(table));
            }
            
            ToTable(table);

            HasKey(o => o.Id);

            var primaryType = typeof (TPrimaryKey);
            if (primaryType.IsValueType)
            {
                if (hasIdentityKey)
                {
                    if (primaryType == typeof (int))
                    {
                        var lambda = ExpressionHelper.CreatePropertySelectorLambda<TEntity, IEntity<int>, int>(o => o.Id);
                        this.Property((Expression<Func<TEntity, int>>) lambda)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    }
                    if (primaryType == typeof (long))
                    {
                        var lambda =
                            ExpressionHelper.CreatePropertySelectorLambda<TEntity, IEntity<long>, long>(o => o.Id);
                        this.Property((Expression<Func<TEntity, long>>) lambda)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    }
                }
            }

            //todo: 添加RowVersion处理并发？
        }
    }

}
