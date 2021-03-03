using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThemePark.Common;

namespace ThemePark.Application.Payment.Dto
{

    /// <summary>
    /// 封装的安字母排序的参数
    /// </summary>
    public class SortedKeyValueData
    {
        private readonly SortedDictionary<string, string> _values = new SortedDictionary<string, string>();

        /// <summary>
        /// 获取参数值,不存在则返回 null
        /// </summary>
        /// <param name="key">参数名</param>
        protected internal virtual string GetValueCore(string key)
        {
            string value;
            return _values.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// 设置参数值,值为 null 则移除该参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        protected internal virtual SortedKeyValueData SetValueCore(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
                _values.Remove(key);
            else
                _values[key] = value;

            return this;
        }

        /// <summary>
        /// 是否包含指定的参数
        /// </summary>
        /// <param name="key">参数名</param>
        public virtual bool HasData(string key)
        {
            return _values.ContainsKey(key);
        }

        /// <summary>
        /// 清空所有参数
        /// </summary>
        public virtual void Clear()
        {
            _values.Clear();
        }

        /// <summary>
        /// 返回所有参数迭代器
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, string>> AsEnumerable()
        {
            return _values.AsEnumerable();
        }

        /// <summary>
        /// 返回所有参数名称
        /// </summary>
        public virtual ICollection<string> Keys()
        {
            return _values.Keys;
        }

        /// <summary>
        /// 返回所有参数个数
        /// </summary>
        public virtual int Count()
        {
            return _values.Count;
        }
    }

    public static class SortedKeyValueDataExtensions
    {
        /// <summary>
        /// 获取 参数值
        /// </summary>
        public static string GetValue(this SortedKeyValueData data, [CallerMemberName] string key = null)
        {
            return data.GetValueCore(key);
        }

        /// <summary>
        /// 设置 参数值
        /// </summary>
        public static TData SetValue<TData>(this TData data, string key, string value)
            where TData : SortedKeyValueData
        {
            data.SetValueCore(key, value);
            return data;
        }

        /// <summary>
        /// 获取 参数值
        /// </summary>
        public static TValue GetValue<TValue>(this SortedKeyValueData data, string key, TValue defaultValue, string dateTimeFormat = null)
            where TValue : struct
        {
            if (typeof(TValue) == typeof(DateTime))
            {
                return (TValue)(object)data.GetValue(key).As(Convert.ToDateTime(defaultValue), dateTimeFormat);
            }

            return data.GetValue(key).As(defaultValue);
        }

        /// <summary>
        /// 获取 参数值
        /// </summary>
        public static TValue? GetValue<TValue>(this SortedKeyValueData data, string key, TValue? defaultValue, string dateTimeFormat = null)
            where TValue : struct
        {
            if (typeof(TValue) == typeof(DateTime))
            {
                return (TValue?)(object)data.GetValue(key).As((DateTime?)(object)defaultValue, dateTimeFormat);
            }

            return data.GetValue(key).As(defaultValue);
        }

        /// <summary>
        /// 设置 参数值
        /// </summary>
        public static TData SetValue<TData, TValue>(this TData data, string key, TValue value, string format = null)
            where TValue : struct
            where TData : SortedKeyValueData
        {
            return data.SetValue(key, string.IsNullOrEmpty(format)
                ? value.ToString()
                : string.Format($"{{0:{format}}}", value));
        }

        /// <summary>
        /// 设置 参数值
        /// </summary>
        public static TData SetValue<TData, TValue>(this TData data, string key, TValue? value, string format = null)
            where TValue : struct
            where TData : SortedKeyValueData
        {
            return data.SetValue(key, string.IsNullOrEmpty(format)
                ? value.ToString()
                : string.Format($"{{0:{format}}}", value));
        }

        /// <summary>
        /// 转换为指定的参数对象
        /// </summary>
        public static TData To<TData>(this SortedKeyValueData data)
            where TData : SortedKeyValueData, new()
        {
            var rs = new TData();

            if (data != null)
            {
                foreach (var item in data.AsEnumerable())
                {
                    rs.SetValue(item.Key, item.Value);
                }
            }

            return rs;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        public static TData Append<TData>(this TData data, SortedKeyValueData append)
            where TData : SortedKeyValueData
        {
            if (append == null) return data;
            return data.Append(append.AsEnumerable());
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        public static TData Append<TData>(this TData data, IEnumerable<KeyValuePair<string, string>> append)
            where TData : SortedKeyValueData
        {
            if (append == null) return data;

            foreach (var item in append)
            {
                data.SetValue(item.Key, item.Value);
            }

            return data;
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        public static TData Append<TData>(this TData data, NameValueCollection append)
            where TData : SortedKeyValueData
        {
            if (append == null) return data;

            foreach (var item in append.AllKeys)
            {
                data.SetValue(item, append.Get(item));
            }

            return data;
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        public static bool AppendJson<TData>(this TData data, string json)
            where TData : SortedKeyValueData
        {
            if (string.IsNullOrWhiteSpace(json)) return true;

            var append = new SortedKeyValueData();
            try
            {
                var jobj = JsonConvert.DeserializeObject<JObject>(json);
                foreach (var item in jobj)
                {
                    append.SetValue(item.Key, (string)item.Value);
                }
            }
            catch
            {
                return false;
            }

            foreach (var item in append.AsEnumerable())
            {
                data.SetValue(item.Key, item.Value);
            }

            return true;
        }


        /// <summary>
        /// 返回 Json 字符串
        /// </summary>
        public static string ToJson(this SortedKeyValueData data)
        {
            return data.ToJson(Formatting.None);
        }

        /// <summary>
        /// 返回 Json 字符串
        /// </summary>
        public static string ToJson(this SortedKeyValueData data, Formatting formatting, params JsonConverter[] converts)
        {
            var obj = new JObject();

            if (data != null)
            {
                foreach (var item in data.AsEnumerable())
                {
                    obj[item.Key] = item.Value;
                }
            }

            return obj.ToString(formatting, converts);
        }

        /// <summary>
        /// 安指定的格式解析日期
        /// </summary>
        public static DateTime As(this string str, DateTime defaultValue, string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return str.As(defaultValue);
            }

            DateTime time;
            return DateTime.TryParseExact(str, format, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out time)
                ? time
                : defaultValue;
        }

        /// <summary>
        /// 安指定的格式解析日期
        /// </summary>
        public static DateTime? As(this string str, DateTime? defaultValue, string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return str.As(defaultValue);
            }

            DateTime time;
            return DateTime.TryParseExact(str, format, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out time)
                ? time
                : defaultValue;
        }
    }
}
