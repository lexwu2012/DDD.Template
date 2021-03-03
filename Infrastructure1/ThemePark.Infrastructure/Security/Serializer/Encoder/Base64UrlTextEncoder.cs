using System;

namespace ThemePark.Infrastructure.Security.Serializer.Encoder
{
    public class Base64UrlTextEncoder : ITextEncoder
    {
        public string Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public byte[] Decode(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            return Convert.FromBase64String(Base64UrlTextEncoder.Pad(text.Replace('-', '+').Replace('_', '/')));
        }

        private static string Pad(string text)
        {
            int count = 3 - (text.Length + 3) % 4;
            if (count == 0)
                return text;
            return text + new string('=', count);
        }
    }
}
