using Abp.Domain.Entities;

namespace ThemePark.Infrastructure.Core
{
    public class Relationship<TPrincipal, TPrincipalPk, TDependent, TDependentPk> : Entity
        where TPrincipal : class, IEntity<TPrincipalPk>, new()
        where TDependent : class, IEntity<TDependentPk>, new()
    {
        public TPrincipalPk PrincipalId { get; set; }

        public virtual TPrincipal Principal { get; set; }

        public TDependentPk DependentId { get; set; }

        public virtual TDependent Dependent { get; set; }
    }

    public class Relationship<TPrincipal, TDependent> : Relationship<TPrincipal, int, TDependent, int> 
        where TPrincipal : class, IEntity<int>, new() 
        where TDependent : class, IEntity<int>, new()
    {

    }
}
