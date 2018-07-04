using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Common.Uow
{
    static class UnitOfWorkHelper
    {
        public static bool HasUnitOfWorkAttribute(MemberInfo memberInfo)
        {
            return memberInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
        }
    }
}
