using System.Reflection;
using Abp.Modules;

namespace ThemePark.Navigation
{
    public class ThemeParkNavigationModule : AbpModule
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
