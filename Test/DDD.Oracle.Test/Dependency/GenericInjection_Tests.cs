﻿using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace DDD.Oracle.Test.Dependency
{
    public class GenericInjection_Tests : OracleTestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Resolve_Generic_Types()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<MyClass>(),
                Component.For(typeof(IEmpty<>)).ImplementedBy(typeof(EmptyImplOne<>))
                );

            var genericObj = LocalIocManager.Resolve<IEmpty<MyClass>>();
            genericObj.GenericArg.GetType().ShouldBe(typeof(MyClass));
        }

        public interface IEmpty<T> where T : class
        {
            T GenericArg { get; set; }
        }

        public class EmptyImplOne<T> : IEmpty<T> where T : class
        {
            public T GenericArg { get; set; }
        }

        public class MyClass
        {

        }
    }
}
