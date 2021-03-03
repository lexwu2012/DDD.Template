using System;
using System.Collections.Generic;
using System.IO;

namespace ThemePark.Infrastructure.Security.Serializer
{
    public class PropertiesSerializer : IDataSerializer<AuthenticationProperties>
    {
        private const int FormatVersion = 1;

        public byte[] Serialize(AuthenticationProperties model)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    Write(writer, model);
                    writer.Flush();
                    return memoryStream.ToArray();
                }
            }
        }

        public AuthenticationProperties Deserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                    return Read(reader);
            }
        }

        public static void Write(BinaryWriter writer, AuthenticationProperties properties)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            writer.Write(FormatVersion);

            writer.Write(properties.Dictionary.Count);
            foreach (var item in properties.Dictionary)
            {
                writer.Write(item.Key);
                writer.Write(item.Value);
            }
        }

        public static AuthenticationProperties Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.ReadInt32() != FormatVersion)
                return null;

            var capacity = reader.ReadInt32();
            var dictionary = new Dictionary<string, string>(capacity);
            for (var index = 0; index != capacity; ++index)
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                dictionary.Add(key, value);
            }
            return new AuthenticationProperties(dictionary);
        }
    }
}
