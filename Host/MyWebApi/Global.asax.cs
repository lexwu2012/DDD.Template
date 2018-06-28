using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Facilities.Logging;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using DDD.Infrastructure.WebApi.Api;
using DDD.Infrastructure.WebApi.Api.Controller;
using DDD.Infrastructure.WebApi.Api.Exceptions;
using DDD.Infrastructure.WebApi.Api.Log;
using DDD.Infrastructure.WebApi.Api.Validation;
using WebApi.HelpPage.Areas.HelpPage.Controllers;

namespace MyWebApi
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
            //iocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog().WithConfig("NLog.config"));

            //注册
            iocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            iocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());

            iocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.Infrastructure.WebApi"));
            //指定解析器
            config.DependencyResolver = new WindsorDependencyResolver(iocManager.IocContainer);

            //添加日志拦截器
            config.MessageHandlers.Add(iocManager.Resolve<HttpMessageLogHandler>());

            //注册helppage
            iocManager.Register<HelpController>(DependencyLifeStyle.Transient);

            //注册过滤器
            InitializeFilters(iocManager,config);


            config.EnsureInitialized();
        }

        private void InitializeFilters(IocManager iocManager,HttpConfiguration config)
        {
            config.Filters.Add(iocManager.Resolve<ApiValidationFilter>(iocManager));
            config.Filters.Add(iocManager.Resolve<ApiExceptionFilter>(iocManager));
        }
    }
}
