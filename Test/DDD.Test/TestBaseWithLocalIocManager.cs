using System;
using System.Reflection;
using DDD.Application.Service;
using DDD.Domain.Common.Repositories;
using DDD.DomainService;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using DDD.Infrastructure.Ioc.Installer;
using Castle.MicroKernel.Registration;

namespace DDD.Test
{
    public abstract class TestBaseWithLocalIocManager : IDisposable
    {
        public readonly IIocManager LocalIocManager;


        public TestBaseWithLocalIocManager()
        {
            LocalIocManager = new IocManager();
            LocalIocManager.IocContainer.Install(new FinderInstaller());
            LocalIocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            LocalIocManager.RegisterAssemblyByConvention(typeof(IRepository).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(DomainServiceBase).Assembly);
            LocalIocManager.RegisterAssemblyByConvention(typeof(IApplicationService).Assembly);
            //LocalIocManager.RegisterAssemblyByConvention(typeof(TestBaseWithLocalIocManager).Assembly);

            LocalIocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.Domain.Common"));
            LocalIocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.Domain.Core"));
            LocalIocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.DomainService"));
            //IocManager.Register<IBlogRepository>();
            //IocManager.Register<IUserRepository>();

        }

        public void Dispose()
        {
            LocalIocManager.Dispose();
        }
    }
}
