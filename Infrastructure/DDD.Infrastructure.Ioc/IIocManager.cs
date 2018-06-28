using System;
using Castle.Windsor;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Ioc
{
    public interface IIocManager: IIocRegistrar, IIocResolver, IDisposable
    {
        IWindsorContainer IocContainer { get; }

    }
}
