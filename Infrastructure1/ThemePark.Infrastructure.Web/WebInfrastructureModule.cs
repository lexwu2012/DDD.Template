using System.Reflection;
using Abp.Modules;
using Abp.Web;

namespace ThemePark.Infrastructure
{
    [DependsOn(typeof(InfrastructureModule), typeof(AbpWebModule))]
    public class WebInfrastructureModule : AbpModule
    {
        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
        
    }
}