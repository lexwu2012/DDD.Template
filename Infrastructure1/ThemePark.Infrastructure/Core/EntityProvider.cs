using System;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Infrastructure.Core
{
    /// <summary>
    /// 创建自动生成Id的实体
    /// </summary>
    public static class EntityProvider
    {
        private static readonly IAbpRedisCacheDatabaseProvider AbpRedisCacheDatabaseProvider;
        private static readonly ITablesNumberCreator TablesNumberCreator;

        private const string IncreaseKey = "IncreasePk";

        private static readonly object LockObj = new object();

        static EntityProvider()
        {
            AbpRedisCacheDatabaseProvider = IocManager.Instance.Resolve<IAbpRedisCacheDatabaseProvider>();
            TablesNumberCreator = IocManager.Instance.Resolve<ITablesNumberCreator>();
        }

        /// <summary>
        /// Generics the primary key.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="pkType">Type of the pk.</param>
        /// <returns>{table No. 4-len}{serial No. n-len}</returns>
        public static object GenericPrimaryKey(Type type, Type pkType)
        {
            int tableNo = TablesNumberCreator.GetTableNo(type);
            if (tableNo <= 0)
            {
                //TODO Cuizj: creat unique value by UniqueCode
                return null;
            }

            bool isInt = false, isLong = false, isString = false;
            if (!(isInt = pkType == typeof(int)) && !(isLong = pkType == typeof(long)) &&
                !(isString = pkType == typeof(string)))
            {
                return null;
            }

            var key = $"{IncreaseKey}_{type.Name}";
            var database = AbpRedisCacheDatabaseProvider.GetDatabase();

            long value = 0;
            if (database.KeyExists(key))
            {
                value = database.HashIncrement(key, true);
            }
            else
            {
                //get lastest value
                lock (LockObj)
                {
                    if (database.KeyExists(key))
                    {
                        value = database.HashIncrement(key, true);
                    }
                    else
                    {
                        object startValue = Nito.AsyncEx.AsyncContext.Run(() => TablesNumberCreator.GetTableLastPk(type));
                        value = Nito.AsyncEx.AsyncContext.Run(() => database.HashIncrementAsync(key, true, (long)startValue + 1));
                        if (isInt || isLong)
                        {
                            var max = isInt ? int.MaxValue.ToString() : long.MaxValue.ToString();
                            max = max.Substring(tableNo.ToString().Length).ToCharArray().Select(o => '9').ToArray().JoinAsString("");
                            var maxValue = isInt ? int.Parse(max) : long.Parse(max);
                            value = value != maxValue ? value % maxValue : value;
                        }
                    }
                }
            }

            var str = tableNo.ToString() + value.ToString();

            return isInt ? int.Parse(str) : isLong ? long.Parse(str) : (object)str;
        }
    }
}
