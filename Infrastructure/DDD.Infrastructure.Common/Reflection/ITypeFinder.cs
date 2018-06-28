using System;

namespace DDD.Infrastructure.Common.Reflection
{
    public interface ITypeFinder
    {
        Type[] Find(Func<Type, bool> predicate);

        Type[] FindAll();

        Type[] GetAllTypes();
    }
}
