using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace ThemePark.ApplicationDto
{
    [DependsOn(typeof(AbpAutoMapperModule))]
    public class ThemeParkApplicationDtoModule : AbpModule
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
