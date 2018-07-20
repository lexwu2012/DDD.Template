using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace DDD.Infrastructure.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        #region Convert

        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <param name="type">转换的目标类型</param>
        public static object As(this string value, Type type)
        {
            object defaultValue = null;
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFrom(value);
                }
                converter = TypeDescriptor.GetConverter(typeof(string));
                if (converter.CanConvertTo(type))
                {
                    return converter.ConvertTo(value, type);
                }
            }
            catch (Exception)
            {
            }
            return defaultValue;
        }

        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        /// <typeparam name="TValue">转换的目标类型</typeparam>
        public static TValue As<TValue>(this string value)
        {
            return As(value, default(TValue));
        }

        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        /// <typeparam name="TValue">转换的目标类型</typeparam>
        public static TValue As<TValue>(this string value, TValue defaultValue)
        {
            return (TValue)value.As(typeof(TValue), defaultValue);
        }

        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        public static object As(this string value, Type destType, object defaultValue)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(destType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFrom(value);
                }

                converter = TypeDescriptor.GetConverter(typeof(string));
                if (converter.CanConvertTo(destType))
                {
                    return converter.ConvertTo(value, destType);
                }
            }
            catch (Exception)
            {
            }
            return defaultValue;
        }

        /// <summary>
        /// 字符串是转换为 <see cref="bool"/>
        /// </summary>
        public static bool AsBool(this string value)
        {
            return As<bool>(value, false);
        }

        /// <summary>
        /// 字符串是转换为 <see cref="bool"/>
        /// </summary>
        public static bool AsBool(this string value, bool defaultValue)
        {
            return As<bool>(value, defaultValue);
        }

        /// <summary>
        /// 字符串是转换为 <see cref="DateTime"/>
        /// </summary>
        public static DateTime AsDateTime(this string value)
        {
            return value.As<DateTime>();
        }

        /// <summary>
        /// 字符串是转换为 <see cref="DateTime"/>
        /// </summary>
        public static DateTime AsDateTime(this string value, DateTime defaultValue)
        {
            return As<DateTime>(value, defaultValue);
        }

        /// <summary>
        /// 字符串是转换为 <see cref="decimal"/>
        /// </summary>
        public static decimal AsDecimal(this string value)
        {
            return value.As<decimal>();
        }

        /// <summary>
        /// 字符串是转换为 <see cref="decimal"/>
        /// </summary>
        public static decimal AsDecimal(this string value, decimal defaultValue)
        {
            return As<decimal>(value, defaultValue);
        }

        /// <summary>
        /// 字符串是转换为 <see cref="float"/>
        /// </summary>
        public static float AsFloat(this string value)
        {
            return value.As<float>();
        }

        /// <summary>
        /// 字符串是转换为 <see cref="float"/>
        /// </summary>
        public static float AsFloat(this string value, float defaultValue)
        {
            return As<float>(value, defaultValue);
        }

        /// <summary>
        /// 字符串是转换为 <see cref="int"/>
        /// </summary>
        public static int AsInt(this string value)
        {
            return value.As<int>();
        }

        /// <summary>
        /// 字符串是转换为 <see cref="int"/>
        /// </summary>
        public static int AsInt(this string value, int defaultValue)
        {
            return As<int>(value, defaultValue);
        }

        /// <summary>
        /// 字符串是转换为 <see cref="int"/>
        /// </summary>
        public static long AsLong(this string value)
        {
            return value.As<long>();
        }

        /// <summary>
        /// 字符串是否可转换为 指定类型
        /// </summary>
        public static bool Is(this string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return ((converter.CanConvertFrom(typeof(string))) && converter.IsValid(value));
        }

        /// <summary>
        /// 字符串是否可转换为 指定类型
        /// </summary>
        public static bool Is<TValue>(this string value)
        {
            return value.Is(typeof(TValue));
        }

        /// <summary>
        /// 字符串是否可转换为 <see cref="bool"/>
        /// </summary>
        public static bool IsBool(this string value)
        {
            return value.Is<bool>();
        }

        /// <summary>
        /// 字符串是否可转换为 <see cref="DateTime"/>
        /// </summary>
        public static bool IsDateTime(this string value)
        {
            return value.Is<DateTime>();
        }

        /// <summary>
        /// 字符串是否可转换为 <see cref="decimal"/>
        /// </summary>
        public static bool IsDecimal(this string value)
        {
            return value.Is<decimal>();
        }

        /// <summary>
        /// 字符串是否为空或空字符串
        /// </summary>
        public static bool IsEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 字符串是否可转换为 <see cref="float"/>
        /// </summary>
        public static bool IsFloat(this string value)
        {
            return value.Is<float>();
        }

        /// <summary>
        /// 字符串是否可转换为 <see cref="int"/>
        /// </summary>
        public static bool IsInt(this string value)
        {
            return value.Is<int>();
        }

        #endregion

        /// <summary>
        /// 确保字符串以指定的字符结尾
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="end">结尾处的字符串</param>
        public static string EnsureEndsWith(this string str, string end)
        {
            return EnsureEndsWith(str, end, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// 确保字符串以指定的字符结尾
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="end">结尾处的字符串</param>
        /// <param name="comparisonType">字符串比较方式</param>
        public static string EnsureEndsWith(this string str, string end, StringComparison comparisonType)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (end.IsNullOrEmpty())
                return str;

            if (str.EndsWith(end.ToString(CultureInfo.InvariantCulture), comparisonType))
            {
                return str;
            }


            return str + end;
        }

        /// <summary>
        /// 确保字符串以指定的字符结尾
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="end">结尾处的字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="culture">区域语言</param>
        public static string EnsureEndsWith(this string str, string end, bool ignoreCase, CultureInfo culture)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (end.IsNullOrEmpty())
                return str;

            if (str.EndsWith(end.ToString(culture), ignoreCase, culture))
            {
                return str;
            }

            return str + end;
        }

        /// <summary>
        /// 确保字符串以指定的字符开头
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="start">开头处的字符串</param>
        public static string EnsureStartsWith(this string str, string start)
        {
            return EnsureStartsWith(str, start, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// 确保字符串以指定的字符开头
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="start">开头处的字符串</param>
        /// <param name="comparisonType">字符串比较方式</param>
        public static string EnsureStartsWith(this string str, string start, StringComparison comparisonType)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (start.IsNullOrEmpty())
                return str;

            if (str.StartsWith(start.ToString(CultureInfo.InvariantCulture), comparisonType))
            {
                return str;
            }

            return start + str;
        }

        /// <summary>
        /// 确保字符串以指定的字符开头
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="start">开头处的字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="culture">区域语言</param>
        public static string EnsureStartsWith(this string str, string start, bool ignoreCase, CultureInfo culture)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (start.IsNullOrEmpty())
                return str;

            if (str.StartsWith(start.ToString(culture), ignoreCase, culture))
            {
                return str;
            }

            return start + str;
        }

        /// <summary>
        /// 指示指定的字符串是 null 还是空字符串。
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 指示指定的字符串是 null 还是空或空白字符串。
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 从字符串的开头截取指定长度的字符
        /// </summary>
        /// <exception cref="ArgumentNullException">如果 <paramref name="str"/> 为空,则抛出异常</exception>
        /// <exception cref="ArgumentException">如果 <paramref name="len"/> 大于字符串的条度，则抛出异常</exception>
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }

        /// <summary>
        /// 替换字符串换行符为系统换行符 <see cref="Environment.NewLine"/>
        /// </summary>
        public static string NormalizeLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }


        /// <summary>
        /// 从字符串的结尾截取指定长度的字符
        /// </summary>
        /// <exception cref="ArgumentNullException">如果 <paramref name="str"/> 为空,则抛出异常</exception>
        /// <exception cref="ArgumentException">如果 <paramref name="len"/> 大于字符串的条度，则抛出异常</exception>
        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// 通过指定的分隔符连接字符串元素
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator">指定分隔符</param>
        /// <returns>
        /// 如果 source 为空 则返回 null
        /// 如果 source 没有任何元素 则返回 <see cref="System.String.Empty"/> 
        /// </returns>
        public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null) return null;

            return string.Join(separator, source);
        }


        /// <summary>
        /// 使用 string.Split 方法以指定的分隔符分隔字符串
        /// </summary>
        /// <param name="str">要分隔的字符串</param>
        /// <param name="separator">分隔符</param>
        public static string[] Split(this string str, string separator)
        {
            return str?.Split(new[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// 使用 string.Split 方法以指定的分隔符分隔字符串
        /// </summary>
        /// <param name="str">要分隔的字符串</param>
        /// <param name="separator">分隔符</param>
        /// <param name="options">分隔选项</param>
        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            return str?.Split(new[] { separator }, options);
        }

        /// <summary>
        /// 使用 string.Split 方法以系统换行符 <see cref="Environment.NewLine"/> 分隔字符串
        /// </summary>
        public static string[] SplitToLines(this string str)
        {
            return str.Split(Environment.NewLine);
        }

        /// <summary>
        /// 使用 string.Split 方法以系统换行符 <see cref="Environment.NewLine"/> 分隔字符串
        /// </summary>
        public static string[] SplitToLines(this string str, StringSplitOptions options)
        {
            return str.Split(Environment.NewLine, options);
        }

        /// <summary>
        /// 根据最大长度截取字符串
        /// </summary>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
                return null;

            if (str.Length <= maxLength)
                return str;

            return str.Left(maxLength);
        }

        /// <summary>
        /// 根据最大长度截取字符串，如果长度超出则加上 <paramref name="postfix"/> 做为后缀
        /// 返回的字符串不会超出最大长度
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="maxLength">截取的长度</param>
        /// <param name="postfix">后缀</param>
        public static string TruncateWithPostfix(this string str, int maxLength, string postfix = "...")
        {
            if (str == null)
                return null;

            if (postfix == null)
                postfix = string.Empty;

            if (str.Length <= maxLength)
                return str;

            if (maxLength <= postfix.Length)
                return postfix.Left(maxLength);

            return str.Left(maxLength - postfix.Length) + postfix;
        }

        /// <summary>
        /// 根据最大字节数截取字符串，如果长度超出则加上 <paramref name="postfix"/> 做为后缀
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="maxBytes">最大字节数</param>
        /// <param name="postfix">后缀</param>
        public static string TruncateWithBytes(this string str, int maxBytes, string postfix = "...")
        {
            if (postfix == null)
                postfix = string.Empty;

            //字符串生成的默认编码的字节长度
            var nameLenth = Encoding.Default.GetByteCount(str);

            //字符串字节长度大于能显示的字节长度，进行截取
            if (nameLenth <= maxBytes) return str;

            //减去将要附加到尾部的"..."的长度,得到要截取的字节长度
            maxBytes = maxBytes - postfix.Length;

            //当前遍历到的字节数,是按displayLength计算字节数的,
            //即汉字算两个字节,英文数字等一个,用来与displayLength比较,好退出循环
            var currentLength = 0;

            //要截取的字节长度,该长度不同于displayLength,这里是Unicode(USC2)编码,
            //不区分汉字还是字母,每个字符占两个字节长度
            var subLength = 0;

            //字符串生成的Unicode(USC2)编码的字节数组
            var strBytes = Encoding.Unicode.GetBytes(str);

            for (; subLength < strBytes.GetLength(0) && currentLength < maxBytes; subLength++)
            {
                //因为Unicode(USC2)编码时,不区分汉字还是字母,每个字符占两个字节长度,
                //这里subLength做下标,为0或每次为偶数时,正好是UCS2编码中两个字节的第一个字节,
                //对于一个英文或数字字符，UCS2编码的第一个字节是相应的ASCII，第二个字节是0，如a的UCS2编码是97 0，而汉字两个字节都不为0
                //除2的余数为0，表示这是每个字符的第一个字节,字母数字等只在这里对CurrentLength加1,汉字等则在第二个字节处判断出后再加1
                if (subLength % 2 == 0)
                {
                    currentLength++;
                }
                else//除2的余数不为0，表明是一个字符的第二个字节,检查字符的第二个字节
                {
                    //汉字需要再加上1,以符合默认编码占两个字节
                    if (strBytes[subLength] > 0)
                    {
                        currentLength++;
                    }
                }
            }
            //如果subLength为奇数时,即截取的最后一个字符，两个字节中只截取了1个即一般,需处理成偶数
            if (subLength % 2 == 1)
            {
                //对字符的第二个字节进行判断(使用自身做下标,因为下标从0开始,实际检查的就是自己后面的一个字节)
                //该UCS2字符是汉字时,第二个字节在默认编码中占1个字节，补全的话，长度超限,所以去掉这个截一半的汉字 
                if (strBytes[subLength] > 0)
                {
                    subLength = subLength - 1;
                }
                else//该UCS2字符是字母或数字,第二个字节在默认编码中不存在，并不占空间,补全该字符
                {
                    subLength = subLength + 1;
                }
            }

            var subStr = Encoding.Unicode.GetString(strBytes, 0, subLength);

            if (postfix.Length > 0)
            {
                subStr += postfix;
            }

            return subStr;
        }


        /// <summary>
        /// 返回可空安全字符串
        /// </summary>
        public static string ToNullSafeString(this object value)
        {
            return value != null ? value.ToString() : string.Empty;
        }


    }
}
