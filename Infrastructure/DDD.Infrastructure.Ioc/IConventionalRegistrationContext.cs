using System.Reflection;

namespace DDD.Infrastructure.Ioc
{
    public interface IConventionalRegistrationContext
    {
        Assembly Assembly { get; }
        
        IIocManager IocManager { get; }
        
    }
}
