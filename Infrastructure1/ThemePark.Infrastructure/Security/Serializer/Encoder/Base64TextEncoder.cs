using System;

namespace ThemePark.Infrastructure.Security.Serializer.Encoder
{
    public class Base64TextEncoder : ITextEncoder
    {
        public string Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public byte[] Decode(string text)
        {
            return Convert.FromBase64String(text);
        }
    }
}
