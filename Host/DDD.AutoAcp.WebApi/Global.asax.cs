using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using DDD.Application.Service;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Repositories;
using DDD.Domain.Core.Uow;
using DDD.Domain.Service;
using DDD.Infrastructure.AutoMapper;
using DDD.Infrastructure.Domain;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Domain.Uow;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using DDD.Infrastructure.WebApi;
using DDD.Infrastructure.WebApi.Api;
using DDD.Infrastructure.WebApi.Api.Controller;
using DDD.Infrastructure.WebApi.Api.Exceptions;
using DDD.Infrastructure.WebApi.Api.Log;
using DDD.Infrastructure.WebApi.Api.Validation;

namespace DDD.AutoAcp.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;
            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes, config);

            var iocManager = new IocManager();

            //指定日志
            iocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog().WithConfig("NLog.config"));
            //iocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

            //开启uow事务
            UnitOfWorkRegistrar.Initialize(iocManager);

            iocManager.IocContainer.Register(
               Component.For(typeof(IDbContextProvider<>)).ImplementedBy(typeof(UnitOfWorkDbContextProvider<>)).LifestyleTransient(),
               Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
               //Component.For<IIocResolver, IocManager>().ImplementedBy<IocManager>().LifestyleSingleton(),
               Component.For<IUnitOfWorkManager>().ImplementedBy<UnitOfWorkManager>().LifestyleSingleton()
               );

            iocManager.Register(typeof(UnitOfWorkInterceptor));

            //注册
            iocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            iocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());
            
            iocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            //注册service 
            iocManager.RegisterAssemblyByConvention(typeof(IAutoMapperModule).Assembly);
            iocManager.RegisterAssemblyByConvention(typeof(IInfrastructureWebApiModule).Assembly);
            iocManager.RegisterAssemblyByConvention(typeof(IInfrastructureDomainModule).Assembly);
            iocManager.RegisterAssemblyByConvention(typeof(IDomainCoreModule).Assembly);
            iocManager.RegisterAssemblyByConvention(typeof(IDomainServiceModule).Assembly);
            iocManager.RegisterAssemblyByConvention(typeof(IAppServiceModule).Assembly);
            iocManager.RegisterAssemblyByConvention(typeof(IocManager).Assembly);
            

            //注册泛型仓储
            using (var repositoryRegistrar = iocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), iocManager, EfAutoRepositoryTypes.Default);
            }

            //指定解析器
            config.DependencyResolver = new WindsorDependencyResolver(iocManager.IocContainer);

            //添加日志拦截器
            config.MessageHandlers.Add(iocManager.Resolve<HttpMessageLogHandler>());

            //指定controller解析器
            config.Services.Replace(typeof(IHttpControllerActivator), new ApiControllerActivator(iocManager));

            //注册helppage
            //iocManager.Register<HelpController>(DependencyLifeStyle.Transient);
            
            //初始化automapper
            InitialAutoMapper(iocManager.IocContainer.Resolve<IAutoMapperInitializer>());

            //注册过滤器
            InitializeFilters(iocManager, config);

            config.EnsureInitialized();
        }

        private void InitializeFilters(IocManager iocManager, HttpConfiguration config)
        {
            config.Filters.Add(iocManager.Resolve<ApiValidationFilter>(iocManager));
            config.Filters.Add(iocManager.Resolve<ApiExceptionFilter>(iocManager));
        }

        private void InitialAutoMapper(IAutoMapperInitializer autoMapperInitializer)
        {
            autoMapperInitializer.Initial();
        }
    }
}
