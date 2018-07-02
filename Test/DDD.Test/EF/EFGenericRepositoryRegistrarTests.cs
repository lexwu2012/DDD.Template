using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using DDD.Domain.BaseEntities;
using DDD.Domain.Common.CustomAttributes;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Core;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Repositories;
using DDD.Domain.Entities;
using DDD.Infrastructure.Ioc.Dependency;
using Shouldly;
using Xunit;

namespace DDD.Test.EF
{
    public class EfGenericRepositoryRegistrarTests : TestBaseWithLocalIocManager
    {
        public EfGenericRepositoryRegistrarTests()
        {
            var fakeBaseDbContextProvider = NSubstitute.Substitute.For<IDbContextProvider<DDDDbContext>>();

            LocalIocManager.IocContainer.Register(
                Component.For<IDbContextProvider<DDDDbContext>>().Instance(fakeBaseDbContextProvider)
                );

            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }
        }

        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            var entity1Repository = LocalIocManager.Resolve<IRepositoryWithTEntityAndTPrimaryKey<User, long>>();
            entity1Repository.Insert(new User
            {
                
            });
            entity1Repository.ShouldNotBe(null);
            (entity1Repository is EfRepositoryBase<DDDDbContext, User, long>).ShouldBeTrue();
        }
    }

    [AutoRepositoryTypes(
            typeof(IRepositoryWithEntity<>),
            typeof(IRepositoryWithTEntityAndTPrimaryKey<,>),
            typeof(MyModuleRepositoryBase<>),
            typeof(MyModuleRepositoryBase<,>)
            )]
    public abstract class MyBaseDbContext : DDDDbContext
    {
        public virtual IDbSet<MyEntity1> MyEntities1 { get; set; }
    }

    public class MyEntity1 : Entity, IAggregateRoot
    {

    }

    public class MyModuleRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<DDDDbContext, TEntity, TPrimaryKey>
           where TEntity : class, IAggregateRoot<TPrimaryKey>
    {
        public MyModuleRepositoryBase()
            : base()
        {
        }
    }

    public class MyModuleRepositoryBase<TEntity> : MyModuleRepositoryBase<TEntity, int>
        where TEntity : class, IAggregateRoot
    {
        public MyModuleRepositoryBase()
            : base()
        {
        }
    }
}
