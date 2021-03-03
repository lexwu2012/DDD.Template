using System;
using System.IO;
using Newtonsoft.Json;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 用于把 Json字符串转换为目标类型
    /// </summary>
    public class StringDataJsonConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return true;
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
                var w = new StringWriter();
                var oldFormatting = serializer.Formatting;
                try
                {
                    serializer.Formatting = Formatting.None;
                    serializer.Serialize(w, value);
                }
                finally
                {
                    serializer.Formatting = oldFormatting;
                }
                writer.WriteValue(w.ToString());
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected token or value . Path:{reader.Path}, Token: {reader.TokenType}, Value: {reader.Value}");

            try
            {
                var w = new StringReader((string)reader.Value);
                return serializer.Deserialize(w, objectType);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Unexpected token or value . Path:{reader.Path}, Token: {reader.TokenType}, Value: {reader.Value}", ex);
            }
        }
    }
}