using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Ioc.Dependency
{
    public static class IocRegistrarExtensions
    {
      
        public static bool RegisterIfNot<T>(this IIocRegistrar iocRegistrar, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where T : class
        {
            if (iocRegistrar.IsRegistered<T>())
            {
                return false;
            }

            iocRegistrar.Register<T>(lifeStyle);
            return true;
        }

       
        public static bool RegisterIfNot(this IIocRegistrar iocRegistrar, Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type))
            {
                return false;
            }

            iocRegistrar.Register(type, lifeStyle);
            return true;
        }

        
        public static bool RegisterIfNot<TType, TImpl>(this IIocRegistrar iocRegistrar, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            if (iocRegistrar.IsRegistered<TType>())
            {
                return false;
            }

            iocRegistrar.Register<TType, TImpl>(lifeStyle);
            return true;
        }


      
        public static bool RegisterIfNot(this IIocRegistrar iocRegistrar, Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type))
            {
                return false;
            }

            iocRegistrar.Register(type, impl, lifeStyle);
            return true;
        }
        
    }
}
