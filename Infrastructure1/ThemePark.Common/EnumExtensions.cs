using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ThemePark.Common
{
    public static class EnumExtensions
    {

        /// <summary>
        /// 获取指定枚举类型的所有值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        public static IList<object> GetEnums(Type enumType)
        {
            var rs = new List<object>();


            if (enumType.IsNullableType())
            {
                enumType = enumType.GetTypeOfNullable();
                rs.Add(null);
            }

            var enums = Enum.GetValues(enumType).Cast<object>();
            rs.AddRange(enums);
            return rs;
        }

        /// <summary>
        /// 获取指定枚举类型的所有值
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        public static IList<TEnum> GetEnums<TEnum>()
        {
            var rs = new List<TEnum>();

            var enumType = typeof(TEnum);
            if (enumType.IsNullableType())
            {
                enumType = enumType.GetTypeOfNullable();
                rs.Add(default(TEnum));
            }

            var enums = Enum.GetValues(enumType).Cast<TEnum>();
            rs.AddRange(enums);
            return rs;
        }

        /// <summary>
        /// 判断位域是否为指定的值
        /// </summary>
        public static bool HasFlag(this Enum self, ulong value)
        {
            return (System.Convert.ToUInt64(self) & value) == value;
        }

        /// <summary>
        /// 判断位域是否为指定的值
        /// </summary>
        public static bool HasFlagX(this Enum self, Enum value)
        {
            return self.HasFlag(System.Convert.ToUInt64(value));
        }

        /// <summary>
        /// 返回枚举包含的位域项
        /// </summary>
        /// <param name="value">要拆分的枚举。</param>
        /// <param name="distinct">是否去除重复的值，当枚举有包含其它枚举的时候。</param>
        public static IEnumerable<TEnum> GetEnumFlags<TEnum>(this Enum value, bool distinct = false)
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentOutOfRangeException(string.Format("类型：{0} 不是枚举类型。", enumType.FullName));

            var values = enumType.GetEnumValues();
            var result = values.Cast<Enum>().Where(value.HasFlagX);

            if (!distinct)
                return result.Cast<TEnum>();

            var orderbyList = result.OrderByDescending(p => p).ToList();
            for (var i = orderbyList.Count - 1; i > 0; i--)
            {
                var item = orderbyList[i];
                if (orderbyList.Any(p => !p.Equals(item) && p.HasFlagX(item)))
                    orderbyList.Remove(item);
            }

            orderbyList.Reverse();
            return orderbyList.Cast<TEnum>();
        }

        /// <summary>
        /// 获取聚合后的枚举值
        /// </summary>
        public static TEnum GetAggregateEnumValue<TEnum>(this IEnumerable<TEnum> source)
            where TEnum : struct
        {
            var value = source.Select(p => System.Convert.ToInt64(p)).Aggregate(0L, (p, t) => p | t);

            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        /// <summary>
        /// 返回枚举定义的说明 没有则返回 null
        /// </summary>
        public static string Description(this Enum val)
        {
            var enumType = val.GetType();
            var text = val.ToString();
            var field = enumType.GetField(text);

            if (field == null)
                return val.ToString();

            var desc = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return desc != null ? desc.Description : text;
        }

        /// <summary>
        /// 获取枚举指定的显示内容
        /// </summary>
        public static object Display(this Enum val, DisplayProperty property)
        {
            var enumType = val.GetType();
            //if val is Flag enum, each item will connect with ","
            var str = val.ToString();

            if (enumType.GetAttribute<FlagsAttribute>() != null && str.Contains(","))
            {
                var array = str.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim());

                var result = array.Aggregate("", (s, s1) =>
                {
                    var f = enumType.GetField(s1);

                    if (f != null)
                    {
                        var text = f.Display(property);
                        return s.IsNullOrEmpty() ? text.ToString() : $"{s},{text}";
                    }

                    return s;
                });

                return result.IsNullOrEmpty() ? null : result;
            }

            var field = enumType.GetField(str);
            if (field != null)
            {
                return field.Display(property);
            }

            return null;
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        public static string DisplayName(this Enum val)
        {
            return val.Display(DisplayProperty.Name) as string;
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        public static string DisplayShortName(this Enum val)
        {
            return val.Display(DisplayProperty.ShortName) as string;
        }

        /// <summary>
        /// 获取枚举分组名称
        /// </summary>
        public static string DisplayGroupName(this Enum val)
        {
            return val.Display(DisplayProperty.GroupName) as string;
        }

        /// <summary>
        /// 获取枚举水印信息
        /// </summary>
        public static string DisplayPrompt(this Enum val)
        {
            return val.Display(DisplayProperty.Prompt) as string;
        }

        /// <summary>
        /// 获取枚举备注
        /// </summary>
        public static string DisplayDescription(this Enum val)
        {
            return val.Display(DisplayProperty.Description) as string;
        }

        /// <summary>
        /// 获取枚举显示排序
        /// </summary>
        public static int? DisplayOrder(this Enum val)
        {
            return val.Display(DisplayProperty.Order) as int?;
        }

        /// <summary>
        /// 返回位域枚举定义的说明
        /// </summary>
        /// <param name="val">枚举值</param>
        /// <param name="split">连接分隔符</param>
        public static string Descriptions(this Enum val, string split = " ")
        {
            if (split == null)
                split = string.Empty;

            if (System.Convert.ToUInt64(val) == 0)
                return val.Description();

            var enumType = val.GetType();
            var text = val.ToString();
            DescriptionAttribute description;
            if (Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
            {
                var vals = Enum.GetValues(enumType);
                var sb = new StringBuilder();

                foreach (Enum e in vals)
                {
                    if (System.Convert.ToUInt64(e) == 0)
                        continue;

                    if (val.HasFlag(e))
                    {
                        description =
                            Attribute.GetCustomAttribute(enumType.GetField(e.ToString()), typeof(DescriptionAttribute))
                                as DescriptionAttribute;

                        sb.Append(description != null ? description.Description : text);
                        sb.Append(split);
                    }
                }
                return sb.ToString().TrimEnd(split.ToCharArray());
            }
            return val.Description();
        }

        /// <summary>
        /// 返回枚举的整型值
        /// </summary>
        public static T To<T>(this Enum val)
            where T : struct
        {
            var longValue = Convert.ToInt64(val);
            var convertType = typeof(T);
            var longType = typeof(long);

            var converter = TypeDescriptor.GetConverter(longType);
            if (converter.CanConvertTo(convertType))
                return (T)converter.ConvertTo(longValue, convertType);

            converter = TypeDescriptor.GetConverter(convertType);
            if (converter.CanConvertFrom(longType))
                return (T)converter.ConvertFrom(longValue);

            throw new InvalidOperationException(string.Format("无法从 {0} 转换为 {1}", val, convertType));
        }

        /// <summary>
        /// 转换字符串为指定的枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要转换的字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <exception cref="ArgumentException"></exception>
        public static T ToEnum<T>(this string value, bool ignoreCase = true)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        /// 转换字符串为指定的枚举，如果失败则使用默认值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要转换的字符串</param>
        /// <param name="defaultValue">默认值，如果转换失败使用</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        public static T TryToEnum<T>(this string value, T defaultValue, bool ignoreCase = true)
            where T : struct
        {
            T val;
            if (Enum.TryParse(value, ignoreCase, out val))
                return val;

            return defaultValue;
        }


        /// <summary>
        /// 转换字符串为指定的枚举，如果失败则使用默认值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要转换的字符串</param>
        /// <param name="defaultValue">默认值，如果转换失败使用</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        public static T? TryToEnum<T>(this string value, T? defaultValue = null, bool ignoreCase = true)
            where T : struct
        {
            T val;
            if (Enum.TryParse(value, ignoreCase, out val))
                return val;

            return defaultValue;
        }

        /// <summary>
        /// 将具有整数值的指定对象转换为枚举成员。
        /// </summary>
        /// <exception cref="ArgumentNullException">value 为 null。</exception>
        /// <exception cref="ArgumentException">
        /// value 不是 System.SByte、System.Int16、System.Int32、System.Int64、System.Byte、System.UInt16、System.UInt32 和 System.UInt64 类型。
        /// </exception>
        public static TEnum ToEnum<TEnum>(this object value)
        {
            var type = typeof(TEnum);

            if (value == null && type.IsNullableType())
                // ReSharper disable once ExpressionIsAlwaysNull
                return (TEnum)value;

            if (type.IsNullableType())
                type = type.GetGenericArguments()[0];

            return (TEnum)Enum.ToObject(type, value);
        }

        /// <summary>
        /// 将具有整数值的指定对象转换为枚举成员。
        /// </summary>
        /// <exception cref="ArgumentNullException">value 为 null。</exception>
        /// <exception cref="ArgumentException">
        /// value 不是 System.SByte、System.Int16、System.Int32、System.Int64、System.Byte、System.UInt16、System.UInt32 和 System.UInt64 类型。
        /// </exception>
        public static string ToEnumDisPlayName<TEnum>(this object value)
        {
            if (value == null)
                return null;

            var type = typeof(TEnum);
            if (type.IsNullableType())
                type = type.GetGenericArguments()[0];

            var enumValue = (Enum)Enum.ToObject(type, value);
            return enumValue.DisplayName();
        }
    }
}
