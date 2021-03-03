using System.Reflection;
using Abp.Modules;

namespace ThemePark.Infrastructure
{
    [DependsOn(typeof(WebInfrastructureModule))]
    public class MvcInfrastructureModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup. 
        ///             Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}