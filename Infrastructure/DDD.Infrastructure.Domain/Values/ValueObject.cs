using System;
using System.Linq;
using System.Reflection;

namespace DDD.Infrastructure.Domain.Values
{
    /// <summary>
    /// 值类型
    /// </summary>
    /// <typeparam name="TValueObject"></typeparam>
    public abstract class ValueObject<TValueObject> : IEquatable<TValueObject>
        where TValueObject : ValueObject<TValueObject>
    {

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var item = obj as ValueObject<TValueObject>;
            return (object)item != null && Equals((TValueObject)item);

        }

        /// <summary>
        /// 判断是否是一样的值
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TValueObject other)
        {
            if ((object)other == null)
            {
                return false;
            }

            var publicProperties = GetType().GetTypeInfo().GetProperties();
            if (!publicProperties.Any())
            {
                return true;
            }

            //比较所有的属性值是否相同
            return publicProperties.All(property => Equals(property.GetValue(this, null), property.GetValue(other, null)));
        }

        public static bool operator ==(ValueObject<TValueObject> x, ValueObject<TValueObject> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(ValueObject<TValueObject> x, ValueObject<TValueObject> y)
        {
            return !(x == y);
        }
    }
}
