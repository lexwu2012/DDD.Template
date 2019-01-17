using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Repositories;
using DDD.Domain.Core.Uow;
using DDD.Domain.Service;
using DDD.Infrastructure.AutoMapper;
using DDD.Infrastructure.Domain;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using Castle.MicroKernel.Registration;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Logger;
using DDD.Infrastructure.Web;
using DDD.Infrastructure.WebApi;

namespace DDD.Application.Service
{
    public class SystemRegister : ISingletonDependency
    {
        public static IIocManager LocalIocManager { get; set; }

        static SystemRegister()
        {
            LocalIocManager = IocManager.Instance;
        }

        public static IIocManager RegisterSystemDependency()
        {

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

            //注册uow拦截器
            LocalIocManager.Register(typeof(UnitOfWorkInterceptor));

            //注册
            LocalIocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            //注册service 
            LocalIocManager.RegisterAssemblyByConvention(typeof(IAutoMapperModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IInfrastructureCommonModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IInfrastructureWebModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IInfrastructureWebApiModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IInfrastructureDomainModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IDomainCoreModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IDomainServiceModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IAppServiceModule).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IocManager).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IInfrastructureLoggerModule).Assembly);

            //注册泛型仓储
            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }

            //注册automapper
            InitialAutoMapper(LocalIocManager);

            return LocalIocManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocManager"></param>
        private static void InitialAutoMapper(IIocManager iocManager)
        {
            var autoMapperInitializer = iocManager.IocContainer.Resolve<IAutoMapperInitializer>();
            autoMapperInitializer.Initialize();
        }
    }
}
