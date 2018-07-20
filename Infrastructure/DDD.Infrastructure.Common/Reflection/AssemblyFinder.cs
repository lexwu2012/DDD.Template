using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDD.Infrastructure.Common.Reflection
{
    public class AssemblyFinder : IAssemblyFinder
    {

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            //foreach (var module in _moduleManager.Modules)
            //{
            //    assemblies.Add(module.Assembly);
            //    assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            //}

            //todo: not to hardcode
            assemblies.Add(Assembly.Load("DDD.Domain.Core"));
            assemblies.Add(Assembly.Load("DDD.Application.Service"));

            return assemblies.Distinct().ToList();
        }
    }
}
