using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.AutoMapper
{
    public interface IAutoMapperInitializer : ISingletonDependency
    {
        void Initialize();
    }
}
