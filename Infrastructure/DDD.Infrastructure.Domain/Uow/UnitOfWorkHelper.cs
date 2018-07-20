using System.Reflection;

namespace DDD.Infrastructure.Domain.Uow
{
    static class UnitOfWorkHelper
    {
        /// <summary>
        /// 查看方法是否有uow特性标志
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static bool HasUnitOfWorkAttribute(MemberInfo memberInfo)
        {
            return memberInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
        }
    }
}
