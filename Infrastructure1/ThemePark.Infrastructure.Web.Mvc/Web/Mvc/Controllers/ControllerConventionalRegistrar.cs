using System.Web.Mvc;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace ThemePark.Infrastructure.Web.Mvc.Controllers
{
    /// <summary>
    /// Registers all MVC Controllers derived from <see cref="Controller"/>.
    /// </summary>
    public class ControllerConventionalRegistrar : IConventionalDependencyRegistrar
    {
        /// <inheritdoc/>
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn<System.Web.Mvc.Controller>()
                    .LifestyleTransient()
                );
        }
    }
}