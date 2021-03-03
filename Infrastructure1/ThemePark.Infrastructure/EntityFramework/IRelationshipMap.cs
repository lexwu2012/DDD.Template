using Abp.Domain.Entities;
using ThemePark.Infrastructure.Core;

namespace ThemePark.Infrastructure.EntityFramework
{
    public interface IRelationshipMap<TEntity, TPrincipal, TPrincipalPk, TDependent, TDependentPk>
        where TEntity : Relationship<TPrincipal, TPrincipalPk, TDependent, TDependentPk>
        where TPrincipal : class, IEntity<TPrincipalPk>, new() 
        where TDependent : class, IEntity<TDependentPk>, new()
    {
        void SetManySide() ;
    }
}
