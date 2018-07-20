using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DDD.Infrastructure.Domain.Events.Factories;
using DDD.Infrastructure.Domain.Events.Handlers;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Domain.Events
{
    //public class EventBusInstaller : IWindsorInstaller
    //{
    //    private readonly IIocResolver _iocResolver;
    //    private IEventBus _eventBus;

    //    public EventBusInstaller(IIocResolver iocResolver)
    //    {
    //        _iocResolver = iocResolver;
    //    }

    //    public void Install(IWindsorContainer container, IConfigurationStore store)
    //    {
    //        container.Register(
    //                Component.For<IEventBus>().UsingFactoryMethod(() => EventBus.Default).LifestyleSingleton()
    //                );
            
    //        _eventBus = container.Resolve<IEventBus>();

    //        container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
    //    }

    //    private void Kernel_ComponentRegistered(string key, IHandler handler)
    //    {
    //        /* This code checks if registering component implements any IEventHandler<TEventData> interface, if yes,
    //         * gets all event handler interfaces and registers type to Event Bus for each handling event.
    //         */
    //        if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(handler.ComponentModel.Implementation))
    //        {
    //            return;
    //        }

    //        var interfaces = handler.ComponentModel.Implementation.GetTypeInfo().GetInterfaces();
    //        foreach (var @interface in interfaces)
    //        {
    //            if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
    //            {
    //                continue;
    //            }

    //            var genericArgs = @interface.GetGenericArguments();
    //            if (genericArgs.Length == 1)
    //            {
    //                _eventBus.Register(genericArgs[0], new IocHandlerFactory(_iocResolver, handler.ComponentModel.Implementation));
    //            }
    //        }
    //    }
    //}
}
