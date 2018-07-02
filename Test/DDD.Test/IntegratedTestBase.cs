using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DDD.Application.Service;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Core;
using DDD.DomainService;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using DDD.Infrastructure.Ioc.Installer;

namespace DDD.Test
{
    public abstract class IntegratedTestBase
    {
        protected IIocManager LocalIocManager { get; }

        public IntegratedTestBase()
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
        }

        protected T Resolve<T>()
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>();
        }

        protected void UsingDbContext(Action<SampleDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<SampleDbContext>())
            {
                //context.DisableAllFilters();
                action(context);
                context.SaveChanges();
            }
        }

        protected T UsingDbContext<T>(Func<SampleDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<SampleDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected void EnsureClassRegistered(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Transient)
        {
            if (!LocalIocManager.IsRegistered(type))
            {
                if (!type.GetTypeInfo().IsClass || type.GetTypeInfo().IsAbstract)
                {
                    throw new Exception("Can not register " + type.Name + ". It should be a non-abstract class. If not, it should be registered before.");
                }

                LocalIocManager.Register(type, lifeStyle);
            }
        }
    }
}
