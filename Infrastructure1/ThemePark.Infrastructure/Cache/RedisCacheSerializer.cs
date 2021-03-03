using System;
using System.Globalization;
using System.IO;
using System.Text;
using Abp.Runtime.Caching.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ThemePark.Infrastructure.Cache
{
    /// <inheritdoc />
    public class RedisCacheSerializer : IRedisCacheSerializer
    {
        private const char TypeSeperator = '|';

        /// <summary>
        /// 签名参数Json内容序列化配置
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings;

        static RedisCacheSerializer()
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

            SerializerSettings = jsSettings;
        }

        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="objbyte">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="M:Abp.Runtime.Caching.Redis.IRedisCacheSerializer.Serialize(System.Object,System.Type)" />
        public object Deserialize(StackExchange.Redis.RedisValue objbyte)
        {
            return DeserializeWithType(objbyte);
        }

        /// <inheritdoc />
        public string Serialize(object value, Type type)
        {
            return SerializeWithType(value, type);
        }

        private static string SerializeWithType(object obj, Type type)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            var buffer = new StringBuilder();
            var writer = new JsonTextWriter(new StringWriter(buffer));
            serializer.Serialize(writer, obj, type);

            var serialized = buffer.ToString();

            return $"{type.AssemblyQualifiedName}{TypeSeperator}{serialized}";
        }

        private static object DeserializeWithType(string serializedObj)
        {
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator);
            var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);
            
            var serializer = JsonSerializer.Create(SerializerSettings);
            var reader = new JsonTextReader(new StringReader(serialized));
            return serializer.Deserialize(reader, type);
        }
    }
}
