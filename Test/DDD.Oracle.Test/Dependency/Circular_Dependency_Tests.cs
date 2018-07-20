﻿using Castle.MicroKernel;
using DDD.Oracle.Test;
using Xunit;

namespace DDD.SqlServer.Test.Dependency
{
    /// <summary>
    /// 循环引用解析异常
    /// </summary>
    public class Circular__Dependency_Tests : OracleTestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Fail_Circular_Constructor_Dependency()
        {
            LocalIocManager.Register<MyClass1>();
            LocalIocManager.Register<MyClass2>();
            LocalIocManager.Register<MyClass3>();

            Assert.Throws<CircularDependencyException>(() => LocalIocManager.Resolve<MyClass1>());
        }

        public class MyClass1
        {
            public MyClass1(MyClass2 obj)
            {

            }
        }

        public class MyClass2
        {
            public MyClass2(MyClass3 obj)
            {

            }
        }

        public class MyClass3
        {
            public MyClass3(MyClass1 obj)
            {

            }
        }
    }
}
