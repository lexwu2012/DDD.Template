using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Castle.MicroKernel.Registration;
using DDD.Application.Service;
using DDD.Domain.Common.Application;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Common.Uow;
using DDD.Domain.Core;
using DDD.Domain.Core.Uow;
using DDD.Domain.Service;
using DDD.DomainService;
using DDD.Infrastructure.AutoMapper.Attributes;
using DDD.Infrastructure.AutoMapper.Extension;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Common.Reflection;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Ioc.Dependency.Registrar;
using DDD.Infrastructure.Ioc.Installer;
using DDD.Test;

namespace DDD.SqlServer.Test
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
            LocalIocManager.RegisterAssemblyByConvention(typeof(AppServiceBase).Assembly);

            LocalIocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            LocalIocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.Domain.Common"));
            LocalIocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.Domain.Core"));
            LocalIocManager.RegisterAssemblyByConvention(Assembly.Load("DDD.DomainService"));

            LocalIocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider<>))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient(),
                Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
                Component.For<IIocResolver, IocManager>().ImplementedBy<IocManager>().LifestyleSingleton()
                );

            //UnitOfWorkRegistrar.Initialize(LocalIocManager);
            InitalAutoMapper();
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

        protected void InitalAutoMapper()
        {
            var list = new List<Action<IMapperConfigurationExpression>>();
            list.Add(ConfigAllMap);

            Action<IMapperConfigurationExpression> action = mapperConfigurationExpression =>
            {
                FindAndAutoMapTypes(mapperConfigurationExpression);
                foreach (var configurator in list)
                {
                    configurator(mapperConfigurationExpression);
                }
            };

            Mapper.Initialize(action);
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var _typeFinder = LocalIocManager.IocContainer.Resolve<ITypeFinder>();
            //var types1 = _typeFinder.GetAllTypes();

            var types = _typeFinder.Find(type =>
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                       typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                       typeInfo.IsDefined(typeof(AutoMapToAttribute));
            }
            );

            foreach (var type in types)
            {
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void ConfigAllMap(IMapperConfigurationExpression configuration)
        {
            configuration.ForAllMaps((map, c) =>
            {
                foreach (var property in map.DestinationType.GetProperties())
                {
                    var attr = property.GetAttribute<MapFromAttribute>();
                    if (attr?.PropertyPath?.Length > 0)
                    {
                        c.ForMember(property.Name, m =>
                        {
                            var path = string.Join(".", attr.PropertyPath);
                            var exp = System.Linq.Dynamic.DynamicExpression.ParseLambda(map.SourceType, null, path);

                            var m0 = m.GetType()
                                .GetProperty("PropertyMapActions", BindingFlags.Instance | BindingFlags.NonPublic)?
                                .GetValue(m) as List<Action<PropertyMap>>;
                            var m1 = typeof(PropertyMap).GetMethod(nameof(PropertyMap.MapFrom));
                            m0?.Add(t => m1.Invoke(t, new object[] { exp }));
                        });
                    }
                }
            });
        }
    }
}
