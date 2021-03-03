using Abp.Domain.Entities;
using ThemePark.Infrastructure.Core;

namespace ThemePark.Infrastructure.EntityFramework
{
    public abstract class RelationshipMap<TEntity, TPrimaryKey, TPrincipal, TPrincipalPk, TDependent, TDependentPk> : 
        BaseMap<TEntity, TPrimaryKey>, IRelationshipMap<TEntity, TPrincipal, TPrincipalPk, TDependent, TDependentPk> 
        where TEntity : Relationship<TPrincipal, TPrincipalPk, TDependent, TDependentPk>, IEntity<TPrimaryKey>, new() 
        where TPrincipal : class, IEntity<TPrincipalPk>, new() 
        where TDependent : class, IEntity<TDependentPk>, new()
    {
        protected RelationshipMap(string table) : base(table)
        {
            SetManySide();
        }

        public void SetManySide()
        {
            this.HasRequired(o => o.Principal).WithMany().HasForeignKey(o => o.PrincipalId);
            this.HasRequired(o => o.Dependent).WithMany().HasForeignKey(o => o.DependentId);
        }
    }

    public abstract class RelationshipMap<TEntity, TPrimaryKey, TPrincipal, TDependent> : 
        RelationshipMap<TEntity, TPrimaryKey, TPrincipal, int, TDependent, int> 
        where TEntity : Relationship<TPrincipal, TDependent>, IEntity<TPrimaryKey>, new() 
        where TPrincipal : class, IEntity<int>, new() 
        where TDependent : class, IEntity<int>, new()
    {
        protected RelationshipMap(string table) : base(table)
        {
            
        }
    }

    public abstract class RelationshipMap<TEntity, TPrincipal, TDependent> :
        RelationshipMap<TEntity, int, TPrincipal, int, TDependent, int>
        where TEntity : Relationship<TPrincipal, TDependent>, IEntity<int>, new()
        where TPrincipal : class, IEntity<int>, new()
        where TDependent : class, IEntity<int>, new()
    {
        protected RelationshipMap(string table) : base(table)
        {

        }
    }
}
