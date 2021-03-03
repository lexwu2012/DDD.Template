using System;
using Newtonsoft.Json;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 用于把 Json字符串转换为 <see cref="PaymentData"/>
    /// </summary>
    public class PaymentDataJsonConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(PaymentData).IsAssignableFrom(objectType);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var signData = (PaymentData)value;

                writer.WriteStartObject();
                foreach (var item in signData.AsEnumerable())
                {
                    writer.WritePropertyName(item.Key);
                    writer.WriteValue(item.Value);
                }
                writer.WriteEndObject();
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.StartObject)
            {
                var signData = (PaymentData)Activator.CreateInstance(objectType);
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            var key = reader.Value.ToString();
                            var val = reader.ReadAsString();
                            signData.SetValue(key, val);
                            continue;
                        case JsonToken.EndObject:
                            return signData;
                        default:
                            continue;
                    }
                }
            }
            throw new JsonSerializationException($"Unexpected end when reading ExpandoObject. Path:{reader.Path}, Token: {reader.TokenType}");
        }
    }
}