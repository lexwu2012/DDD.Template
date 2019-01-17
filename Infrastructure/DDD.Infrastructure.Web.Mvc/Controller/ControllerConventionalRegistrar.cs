using System.Reflection;
using Castle.MicroKernel.Registration;
using DDD.Infrastructure.Ioc;

namespace DDD.Infrastructure.Web.Mvc.Controller
{
    public class ControllerConventionalRegistrar : IConventionalDependencyRegistrar
    {
        /// <inheritdoc/>
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                       .BasedOn<System.Web.Mvc.Controller>()
                       .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                       .LifestyleTransient()
            );
        }
    }
}
