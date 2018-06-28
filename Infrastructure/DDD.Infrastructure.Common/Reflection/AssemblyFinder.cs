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

            assemblies.Add(Assembly.Load("DDD.Application.Dto"));
            assemblies.Add(Assembly.Load("DDD.MyApplication"));

            return assemblies.Distinct().ToList();
        }
    }
}
