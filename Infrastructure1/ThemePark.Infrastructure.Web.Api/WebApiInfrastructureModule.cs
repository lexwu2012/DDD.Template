using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Abp.Modules;
using ThemePark.Infrastructure.Web.Api;
using ThemePark.Infrastructure.Web.Api.Controllers;
using ThemePark.Infrastructure.Web.Api.Exceptions;
using ThemePark.Infrastructure.Web.Api.Log;
using ThemePark.Infrastructure.Web.Api.Uow;
using ThemePark.Infrastructure.Web.Api.Validation;

namespace ThemePark.Infrastructure
{
    [DependsOn(typeof(WebInfrastructureModule))]
    public class WebApiInfrastructureModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup. 
        ///             Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
            var config = GlobalConfiguration.Configuration;
            IocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());

            config.DependencyResolver = new WindsorDependencyResolver(IocManager.IocContainer);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            var config = GlobalConfiguration.Configuration;
            config.MessageHandlers.Add(IocManager.Resolve<HttpMessageLogHandler>());
        }

        /// <summary>
        /// This method is called lastly on application startup.
        /// </summary>
        public override void PostInitialize()
        {
            var config = GlobalConfiguration.Configuration;
            config.Services.Replace(typeof (IHttpControllerActivator), new ApiControllerActivator(IocManager));

            InitializeFilters(config);

            config.EnsureInitialized();
        }

        private void InitializeFilters(HttpConfiguration config)
        {
            config.Filters.Add(IocManager.Resolve<AbpApiValidationFilter>());
            config.Filters.Add(IocManager.Resolve<AbpApiUowFilter>());
            config.Filters.Add(IocManager.Resolve<ApiExceptionFilter>());
        }
    }
}