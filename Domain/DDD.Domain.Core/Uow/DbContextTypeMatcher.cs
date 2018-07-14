using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.CustomAttributes;
using DDD.Domain.Common.Uow;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Core.Uow
{
    public abstract class DbContextTypeMatcher<TBaseDbContext> : IDbContextTypeMatcher, ISingletonDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly Dictionary<Type, List<Type>> _dbContextTypes;

        protected DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _dbContextTypes = new Dictionary<Type, List<Type>>();
        }

        public void Populate(Type[] dbContextTypes)
        {
            foreach (var dbContextType in dbContextTypes)
            {
                var types = new List<Type>();

                AddWithBaseTypes(dbContextType, types);

                foreach (var type in types)
                {
                    Add(type, dbContextType);
                }
            }
        }


        public virtual Type GetConcreteType(Type sourceDbContextType)
        {
            if (!sourceDbContextType.GetTypeInfo().IsAbstract)
            {
                return sourceDbContextType;
            }

            //Get possible concrete types for given DbContext type
            var allTargetTypes = _dbContextTypes.GetOrDefault(sourceDbContextType);

            if (allTargetTypes.IsNullOrEmpty())
            {
                throw new Exception("Could not find a concrete implementation of given DbContext type: " + sourceDbContextType.AssemblyQualifiedName);
            }

            if (allTargetTypes.Count == 1)
            {
                //Only one type does exists, return it
                return allTargetTypes[0];
            }

            if (_currentUnitOfWorkProvider.Current == null)
            {
                throw new Exception("GetConcreteType method should be called in a UOW.");
            }

            return GetDefaultDbContextType(allTargetTypes, sourceDbContextType);
        }


        private static Type GetDefaultDbContextType(List<Type> dbContextTypes, Type sourceDbContextType)
        {
            var filteredTypes = dbContextTypes
                .Where(type => !type.GetTypeInfo().IsDefined(typeof(AutoRepositoryTypesAttribute), true))
                .ToList();

            if (filteredTypes.Count == 1)
            {
                return filteredTypes[0];
            }

            throw new Exception(string.Format(
                "Found more than one concrete type for given DbContext Type ({0}). Found types: {2}.",
                sourceDbContextType,
                dbContextTypes.Select(c => c.AssemblyQualifiedName).JoinAsString(", ")
                ));
        }

        private static void AddWithBaseTypes(Type dbContextType, List<Type> types)
        {
            types.Add(dbContextType);
            if (dbContextType != typeof(TBaseDbContext))
            {
                AddWithBaseTypes(dbContextType.GetTypeInfo().BaseType, types);
            }
        }

        private void Add(Type sourceDbContextType, Type targetDbContextType)
        {
            if (!_dbContextTypes.ContainsKey(sourceDbContextType))
            {
                _dbContextTypes[sourceDbContextType] = new List<Type>();
            }

            _dbContextTypes[sourceDbContextType].Add(targetDbContextType);
        }
    }
}
