using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DDD.Infrastructure.Common.Extensions;

namespace DDD.Infrastructure.Common.Reflection
{
    public class TypeFinder : ITypeFinder
    {
        private readonly IAssemblyFinder _assemblyFinder;
        private readonly object _syncObj = new object();
        private Type[] _types;

        public TypeFinder(IAssemblyFinder assemblyFinder)
        {
            _assemblyFinder = assemblyFinder;
        }

        public Type[] Find(Func<Type, bool> predicate)
        {
            return GetAllTypes().Where(predicate).ToArray();
        }

        public Type[] FindAll()
        {
            return GetAllTypes().ToArray();
        }

        public Type[] GetAllTypes()
        {
            if (_types == null)
            {
                lock (_syncObj)
                {
                    if (_types == null)
                    {
                        _types = CreateTypeList().ToArray();
                    }
                }
            }

            return _types;
        }

        private List<Type> CreateTypeList()
        {
            var allTypes = new List<Type>();

            var assemblies = _assemblyFinder.GetAllAssemblies().Distinct();

            foreach (var assembly in assemblies)
            {
                try
                {
                    Type[] typesInThisAssembly;

                    try
                    {
                        typesInThisAssembly = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        typesInThisAssembly = ex.Types;
                    }

                    if (typesInThisAssembly.IsNullOrEmpty())
                    {
                        continue;
                    }

                    allTypes.AddRange(typesInThisAssembly.Where(type => type != null));
                }
                catch (Exception ex)
                {
                }
            }

            return allTypes;
        }
    }
}
