using System;
using System.Collections.Concurrent;

namespace ThemePark.Infrastructure.EntityFramework
{
    /// <summary>
    /// DynamicFilter条件开关配置
    /// </summary>
    public sealed class DynamicFilterConditions
    {
        private static Lazy<DynamicFilterConditions> _single = new Lazy<DynamicFilterConditions>(() => new DynamicFilterConditions());

        private ConcurrentDictionary<string, DynamicFilterCondition> _internalDic;

        /// <summary>
        /// 动态参数实例
        /// </summary>
        /// <value>The instance.</value>
        public static DynamicFilterConditions Instance => _single.Value;

        private DynamicFilterConditions()
        {
            _internalDic = new ConcurrentDictionary<string, DynamicFilterCondition>();
        }

        public void Add(string filterName, Func<bool> condition)
        {
            var value = new DynamicFilterCondition(filterName, condition);
            if (!_internalDic.TryAdd(filterName, value))
            {
                throw new Exception($"DynamicFilterConditions operation failed: Add the filter {filterName}");
            }
        }

        public bool NotExistsOrConditionIsEnable(string filterName)
        {
            if (!_internalDic.ContainsKey(filterName))
            {
                return true;
            }

            DynamicFilterCondition value;

            if (!_internalDic.TryGetValue(filterName, out value))
            {
                throw new Exception($"DynamicFilterConditions operation failed: Get the filter {filterName}");
            }

            return value.Condition.Invoke();
        }
    }

    class DynamicFilterCondition
    {
        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public DynamicFilterCondition(string filterName, Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            FilterName = filterName;
            Condition = condition;
        }

        public string FilterName { get; set; }

        public Func<bool> Condition { get; set; }
    }
}
