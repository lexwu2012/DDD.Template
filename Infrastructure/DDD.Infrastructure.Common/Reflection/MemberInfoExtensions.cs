using System;
using System.Linq;
using System.Reflection;

namespace DDD.Infrastructure.Common.Reflection
{
    public static class MemberInfoExtensions
    {
        public static TAttribute GetSingleAttributeOrNull<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
            where TAttribute : Attribute
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            var attrs = memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).ToArray();
            if (attrs.Length > 0)
            {
                return (TAttribute)attrs[0];
            }

            return default(TAttribute);
        }


        public static TAttribute GetSingleAttributeOfTypeOrBaseTypesOrNull<TAttribute>(this Type type, bool inherit = true)
            where TAttribute : Attribute
        {
            var attr = type.GetTypeInfo().GetSingleAttributeOrNull<TAttribute>();
            if (attr != null)
            {
                return attr;
            }

            if (type.GetTypeInfo().BaseType == null)
            {
                return null;
            }

            return type.GetTypeInfo().BaseType.GetSingleAttributeOfTypeOrBaseTypesOrNull<TAttribute>(inherit);
        }
    }
}
