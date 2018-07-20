using System.Data.Entity;
using Castle.MicroKernel.Registration;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Domain.CustomAttributes;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Ioc.Dependency;
using Shouldly;
using Xunit;

namespace DDD.SqlServer.Test.EF
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

            //var fakeBaseDbContextProvider = NSubstitute.Substitute.For<IDbContextProvider<MyBaseDbContext>>();

            //LocalIocManager.IocContainer.Register(
            //    Component.For<IDbContextProvider<MyBaseDbContext>>().Instance(fakeBaseDbContextProvider),
            //    Component.For<IDbContextEntityFinder>().ImplementedBy<EfDbContextEntityFinder>().LifestyleTransient(),
            //    Component.For<EfGenericRepositoryRegistrar>().LifestyleTransient()
            //    );

            //using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            //{
            //    repositoryRegistrar.Object.RegisterForDbContext(typeof(MyBaseDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            //}
        }

        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            var entity1Repository = LocalIocManager.Resolve<IRepositoryWithTEntityAndTPrimaryKey<User, long>>();
            
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
        public string Remark { get; set; }
    }

    public class MyModuleRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<DDDDbContext, TEntity, TPrimaryKey>
           where TEntity : class, IAggregateRoot<TPrimaryKey>
    {
        public MyModuleRepositoryBase(IDbContextProvider<DDDDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    public class MyModuleRepositoryBase<TEntity> : MyModuleRepositoryBase<TEntity, int>
        where TEntity : class, IAggregateRoot
    {
        public MyModuleRepositoryBase(IDbContextProvider<DDDDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
