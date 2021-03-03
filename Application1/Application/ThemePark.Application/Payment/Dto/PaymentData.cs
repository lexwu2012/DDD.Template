using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Abp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ThemePark.Infrastructure.Security;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 收银台签名参数基类
    /// </summary>
    [JsonConverter(typeof(PaymentDataJsonConverter))]
    public class PaymentData : SortedKeyValueData
    {
        /// <summary>
        /// 日期字符串格式
        /// </summary>
        public const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 签名参数Json内容序列化配置
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings;

        static PaymentData()
        {
            var jsSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
            };

            jsSettings.Converters.Add(new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss",
                DateTimeStyles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal
            });
            //jsSettings.Converters.Add(new StringEnumConverter());

            SerializerSettings = jsSettings;
        }
    }

    /// <summary/>
    public static class SignDataExtensions
    {
        /// <summary>
        /// 是否为成功
        /// </summary>
        public static bool IsOk(this PaymentResult data)
        {
            return data.Code == PaymentResultCode.Ok;
        }

        /// <summary>
        /// 设置参数JSON字符串值
        /// </summary>
        public static void SetJsonValue(this PaymentData data, object value, [CallerMemberName]string key = null)
        {
            if (value == null)
                data.SetValue(key, null);
            else
            {
                var serializer = JsonSerializer.Create(PaymentData.SerializerSettings);
                var buffer = new StringBuilder();
                var writer = new JsonTextWriter(new StringWriter(buffer));
                writer.QuoteChar = '\'';
                serializer.Serialize(writer, value);

                data.SetValue(key, buffer.ToString());
            }
        }

        /// <summary>
        /// 获取参数JSON值
        /// </summary>
        public static T GetJsonValue<T>(this PaymentData data, T defaultValue, [CallerMemberName]string key = null)
        {
            var val = data.GetValue(key);
            if (!val.IsNullOrEmpty())
            {
                try
                {
                    var serializer = JsonSerializer.Create(PaymentData.SerializerSettings);
                    var reader = new JsonTextReader(new StringReader(val));
                    return serializer.Deserialize<T>(reader);
                }
                catch { }
            }
            return defaultValue;
        }

        /// <summary>
        /// 对参数进行 RSA 签名
        /// </summary>
        /// <param name="data">参数对象</param>
        /// <param name="privateKey">商户密钥</param>
        public static string MakeRsaSign(this PaymentRequest data, string privateKey)
        {
            if (string.IsNullOrEmpty(privateKey)) return null;

            var str = GetSignData(data);
            return RsaHelper.Sign(str, privateKey);
        }

        /// <summary>
        /// 检查 RSA 签名是否正确
        /// </summary>
        /// <param name="data">参数对象</param>
        /// <param name="publicKey">商户密钥</param>
        public static bool CheckRsaSign(this PaymentData data, string publicKey)
        {
            if (string.IsNullOrEmpty(publicKey)) return false;

            var str = GetSignData(data);
            return RsaHelper.Verify(str, data.GetValue(nameof(PaymentRequest.Sign)), publicKey);
        }

        /// <summary>
        /// 获取需要签名的数据
        /// </summary>
        public static string GetSignData(this PaymentData data)
        {
            var str = new StringBuilder();
            foreach (var item in data.AsEnumerable())
            {
                // 如果参数为签名字段或值为空则忽略
                if (item.Key == nameof(PaymentRequest.Sign) || string.IsNullOrEmpty(item.Value)) continue;
                if (str.Length > 0) str.Append("&");

                str.AppendFormat("{0}={1}", item.Key, item.Value);
            }
            return str.ToString();
        }
    }
}