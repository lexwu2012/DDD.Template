using Abp.Modules;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using ThemePark.Application;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure.Services;

namespace ThemePark.Services.Host
{
    [DependsOn(typeof(ThemeParkDataModule), typeof(ThemeParkApplicationModule))]
    public class ThemeParkServicesModule : AbpModule
    {
        public ServiceContext ServiceContext { get; private set; }

        public override void PreInitialize()
        {
            base.PreInitialize();

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            //register all wcf dependencies
            IocManager.IocContainer.Register(
                Component.For<ServiceFinder>().ImplementedBy<ServiceFinder>().LifestyleSingleton(),
                Component.For<IServiceManager>().ImplementedBy<ServiceManager>().LifestyleSingleton(),
                Component.For<ServiceContext, WcfServiceContext>().ImplementedBy<WcfServiceContext>().LifestyleSingleton());

            IocManager.IocContainer.Register(
                Component.For<ISessionProvider>().ImplementedBy<CallContextSessionProvider>().LifestyleSingleton()
                //,Component.For<IAbpSession>().ImplementedBy<WcfSession>()
                //    .UsingFactoryMethod(kernel =>
                //    {
                //        Console.WriteLine("create session domain: " + AppDomain.CurrentDomain.FriendlyName);
                //        var id = CallContext.LogicalGetData(SessionKey.UserId);
                //        return kernel.Resolve<ISessionProvider>().GetCurrentSession();
                //    })
                //    .LifestylePerWcfSession()

                //TODO: IAbpSession must be singleton, IAbpSession use by Property Injection in other Component for Component.Initializer()
                , Component.For<IAbpSession>().ImplementedBy<WcfSession>().LifestyleSingleton());
        }

        public override void Initialize()
        {
            base.Initialize();

            //register channelfactory
            ServiceContext = IocManager.Resolve<ServiceContext>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            ServiceContext.Current.ServiceManager.StartAllServices();
        }

        public override void Shutdown()
        {
            ServiceContext.Current.ServiceManager.StopAllServices();

            base.Shutdown();
        }
    }
}
