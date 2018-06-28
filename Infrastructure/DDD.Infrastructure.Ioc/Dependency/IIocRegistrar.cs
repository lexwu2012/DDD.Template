using System;
using System.Reflection;

namespace DDD.Infrastructure.Ioc.Dependency
{
    public interface IIocRegistrar
    {
        /// <summary>
        /// Adds a dependency registrar for conventional registration.
        /// </summary>
        /// <param name="registrar">dependency registrar</param>
        void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar);

        void RegisterAssemblyByConvention(Assembly assembly);

        void Register<T>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where T : class;

        void Register(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        void Register<TType, TImpl>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType;

        void Register(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        bool IsRegistered(Type type);

        bool IsRegistered<TType>();
    }
}
