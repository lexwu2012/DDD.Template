using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using DDD.Application.Service;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Uow;
using DDD.Domain.Service;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using DDD.Infrastructure.Ioc.Installer;
using System.Reflection;

namespace DDD.Schedule.App_Start
{
    public class Startup
    {
        public static void Configuration()
        {
            var iocManager = new IocManager();

            RegisterAssemblsByConvention(iocManager);
            RegisterCommon(iocManager);
        }

        private static void RegisterAssemblsByConvention(IocManager iocManager)
        {
            iocManager.IocContainer.Install(new FinderInstaller());
            iocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            iocManager.RegisterAssemblyByConvention(typeof(IRepository).Assembly);
            //todo: replace hard code
            iocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.Domain.Core"));
            iocManager.RegisterAssemblyByConvention(typeof(DomainServiceBase).Assembly);
            iocManager.RegisterAssemblyByConvention(typeof(AppServiceBase).Assembly);

            iocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private static void RegisterCommon(IocManager iocManager)
        {
            iocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog().WithConfig("NLog.config"));

            iocManager.IocContainer.Register(
               Component.For(typeof(IDbContextProvider<>))
                   .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                   .LifestyleTransient(),
               Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
               Component.For<IIocResolver, IocManager>().ImplementedBy<IocManager>().LifestyleSingleton()
               );

            //注册uow拦截器
            //UnitOfWorkRegistrar.Initialize(LocalIocManager);

            //InitalAutoMapper();
        }
    }
}
