using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using DDD.Application.Service;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Uow;
using DDD.Domain.Service;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using DDD.Infrastructure.Ioc.Installer;

namespace DDD.SqlServer.Test
{
    public abstract class TestBaseWithLocalIocManager : IDisposable
    {
        public readonly IIocManager LocalIocManager;


        public TestBaseWithLocalIocManager()
        {
            LocalIocManager = new IocManager();


            /*
             * 需要注意注册的次序！
             * 先注册拦截器
             */

            //开启uow事务
            UnitOfWorkRegistrar.Initialize(LocalIocManager);

            LocalIocManager.IocContainer.Register(
              Component.For(typeof(IDbContextProvider<>)).ImplementedBy(typeof(UnitOfWorkDbContextProvider<>)).LifestyleTransient(),
              Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
              Component.For<IIocResolver, IocManager>().ImplementedBy<IocManager>().LifestyleSingleton(),
              Component.For<IUnitOfWorkManager>().ImplementedBy<UnitOfWorkManager>().LifestyleSingleton()
              );

            LocalIocManager.Register(typeof(UnitOfWorkInterceptor), DependencyLifeStyle.Singleton);

            //IocManager.Register<HelpController>(DependencyLifeStyle.Transient);

            LocalIocManager.IocContainer.Install(new FinderInstaller());
            LocalIocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            LocalIocManager.RegisterAssemblyByConvention(typeof(IRepository).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(DomainServiceBase).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(AppServiceBase).Assembly);
            //LocalIocManager.RegisterAssemblyByConvention(typeof(TestBaseWithLocalIocManager).Assembly);

            LocalIocManager.RegisterAssemblyByConvention(typeof(IDomainCoreModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IDomainServiceModule).Assembly);
        }

        public void Dispose()
        {
            LocalIocManager.Dispose();
        }
    }
}
